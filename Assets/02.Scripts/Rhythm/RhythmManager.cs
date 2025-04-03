using System;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    [SerializeField] private EventReference eventReference;
    public bool IsTest = true;
    public bool IsPlaying = false;
    public float BPM;
    private EventInstance musicInstance;
    private GCHandle timelineHandle;

    public float CurrentTimelineTime { get; private set; } = 0f;

    public static event Action<float> OnBeat; // beat 시간(초) 전달
    public static event Action<string> OnMarker;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        TimelineInfo info = new TimelineInfo();
        timelineHandle = GCHandle.Alloc(info);

        musicInstance = RuntimeManager.CreateInstance(eventReference);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicInstance.setCallback(FMODCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        if (IsTest)
            Play();
    }
    public float MusicStartTime { get; private set; } = -1f;
    public void Play()
    {
        if (!musicInstance.isValid()) return;
        if (!IsPlaying)
        {
            IsPlaying = true;
            musicInstance.start();
            musicInstance.getTimelinePosition(out int ms);
            MusicStartTime = Time.time - (ms / 1000f);
        }
            
    }

    public float GetCurrentMusicTime()
    {
        if (!musicInstance.isValid()) return 0f;
        musicInstance.getTimelinePosition(out int ms);
        CurrentTimelineTime = ms / 1000f;
        return CurrentTimelineTime;
    }

    private void OnDestroy()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.setCallback(null);
            musicInstance.release();
        }

        if (timelineHandle.IsAllocated)
            timelineHandle.Free();
    }

    // 콜백용 구조체
    class TimelineInfo
    {
        public int currentBar = 0;
        public string lastMarker = "";
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static FMOD.RESULT FMODCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        instance.getUserData(out var dataPtr);
        if (dataPtr == IntPtr.Zero) return FMOD.RESULT.OK;

        var handle = GCHandle.FromIntPtr(dataPtr);
        if (!handle.IsAllocated || handle.Target == null) return FMOD.RESULT.OK;

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                var beat = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                Instance.BPM = beat.tempo;

                float beatTime = beat.position / 1000f;
                Instance.CurrentTimelineTime = beatTime;
                OnBeat?.Invoke(beatTime); // 정확한 beat 시간 전달
                break;

            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                var marker = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                OnMarker?.Invoke(marker.name);
                break;
        }

        return FMOD.RESULT.OK;
    }
}
