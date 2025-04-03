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
    [Space(10)]
    //=====
    [Header("테스트용 (정식땐 지워야함!!)")]
    [Tooltip("체크시 음악이 바로 시작됩니다")]public bool IsTest = true;
    //=====
    [Space(10)]


    private EventInstance musicInstance;
    private GCHandle timelineHandle;
    private bool _hasDestroyed;
    // 콜백으로 받을 데이터 구조
    class TimelineInfo
    {
        public int currentBar = 0;
        public string lastMarker = "";
    }

    public static event Action<int> OnBeat;
    public static event Action<string> OnMarker;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    void Start()
    {
        TimelineInfo info = new TimelineInfo();
        timelineHandle = GCHandle.Alloc(info);

        musicInstance = RuntimeManager.CreateInstance(eventReference);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicInstance.setCallback(FMODCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);


        if (IsTest)
        {
            musicInstance.start();
        }
    }

    void OnDestroy()
    {
        if (_hasDestroyed) return;
        _hasDestroyed = true;

        if (timelineHandle.IsAllocated)
            timelineHandle.Free();

        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }


    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT FMODCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        instance.getUserData(out var dataPtr);
        if (dataPtr == IntPtr.Zero) return FMOD.RESULT.OK;

        var handle = GCHandle.FromIntPtr(dataPtr);

        if (!handle.IsAllocated || handle.Target == null)
        {
            return FMOD.RESULT.OK;
        }
        var info = (TimelineInfo)handle.Target;

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                var beat = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                info.currentBar = beat.bar;

                Debug.Log($"Beat: bar={beat.bar}, position={beat.position}, tempo={beat.tempo}");
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
