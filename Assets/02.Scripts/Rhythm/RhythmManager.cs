using System;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    [Header("FMOD 이벤트")]
    [SerializeField] private EventReference eventReference;

    private EventInstance musicInstance;
    private GCHandle timelineHandle;

    // 콜백으로 받을 데이터 구조
    class TimelineInfo
    {
        public int currentBar = 0;
        public string lastMarker = "";
    }

    // 유니티 이벤트
    public static event Action<int> OnBeat;         // bar 번호 전달
    public static event Action<string> OnMarker;    // marker name 전달

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        RhythmManager.OnMarker += markerName => {
            Debug.Log($"📍 마커 도착: {markerName}");
        };
    }

    void Start()
    {
        // 데이터 구조 생성 + 고정
        TimelineInfo info = new TimelineInfo();
        timelineHandle = GCHandle.Alloc(info);

        // 인스턴스 생성 및 콜백 설정
        musicInstance = RuntimeManager.CreateInstance(eventReference);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicInstance.setCallback(FMODCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        // 재생 시작
        musicInstance.start();
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();

        if (timelineHandle.IsAllocated)
            timelineHandle.Free();
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT FMODCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        instance.getUserData(out var dataPtr);
        if (dataPtr == IntPtr.Zero) return FMOD.RESULT.OK;

        var handle = GCHandle.FromIntPtr(dataPtr);
        var info = (TimelineInfo)handle.Target;
        Debug.Log("Beat");
        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                var beat = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                info.currentBar = beat.bar;
                OnBeat?.Invoke(beat.bar);
                break;

            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                var marker = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                info.lastMarker = marker.name;
                OnMarker?.Invoke(marker.name);
                break;
        }

        return FMOD.RESULT.OK;
    }
}
