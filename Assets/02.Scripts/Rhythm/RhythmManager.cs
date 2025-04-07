using System;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using static RhythmEvents;
using System.Collections.Generic;

enum NoteTriggerState
{
    None,
    Previewed,
    Triggered
}

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    public bool IsTest = true;

    public bool IsPlaying = false;
    private float _bpm;
    public float BPM
    {
        get => _bpm;
        private set => _bpm = value;
    }

    [SerializeField] private float previewLeadTimeInBeat = 4f;
    [SerializeField] private RhythmPatternSO[] stageNotes;
    [SerializeField] private EventReference[] musicTracks;
    public float beatDuration => 60f / _bpm;
    public float CurrentTimelineTime { get; private set; } = 0f;
    public float MusicStartTime { get; private set; } = -1f;

    private int _previewIndex = 0;
    private EventInstance _musicInstance;
    private GCHandle _timelineHandle;
    private int _stageMusicIndex;
    private List<NoteTriggerState> _noteStates;

    public int StageMusicIndex
    {
        get => _stageMusicIndex;
        private set => _stageMusicIndex = value;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        if (IsTest)
        {
            Play();
        }

    }

    void Update()
    {
        if (!IsPlaying) return;

        float currentTime = GetCurrentMusicTime();

        for (int i = 0; i < stageNotes[_stageMusicIndex].notes.Count; i++)
        {
            var note = stageNotes[_stageMusicIndex].notes[i];
            float noteTime = note.beat * beatDuration;
            float previewTime = noteTime - (previewLeadTimeInBeat * beatDuration);

            switch (_noteStates[i])
            {
                case NoteTriggerState.None:
                    if (currentTime >= previewTime)
                    {
                        _noteStates[i] = NoteTriggerState.Previewed;
                        InvokeOnNotePreview(note);
                        Debug.Log($"[미리보기] 키: {note.expectedKey}, 비트: {note.beat}");
                    }
                    break;

                case NoteTriggerState.Previewed:
                    if (currentTime >= noteTime)
                    {
                        _noteStates[i] = NoteTriggerState.Triggered;
                        InvokeOnNote(note);
                        Debug.Log($"[노트 발동] 키: {note.expectedKey}, 비트: {note.beat}");
                    }
                    break;
            }
        }
    }

    public void OnLoadedStage(int stageMusicIndex)
    {
        _stageMusicIndex = stageMusicIndex;

        Play();
    }
    public void Play()
    {

        //음악을 종료하고
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.release();
            IsPlaying = false;
        }
        if (_timelineHandle.IsAllocated)
        {
            _timelineHandle.Free();
        }

        TimelineInfo info = new TimelineInfo();
        _timelineHandle = GCHandle.Alloc(info);

        _musicInstance = RuntimeManager.CreateInstance(musicTracks[_stageMusicIndex]);
        _musicInstance.setUserData(GCHandle.ToIntPtr(_timelineHandle));
        _musicInstance.setCallback(FMODCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        _previewIndex = 0;
        _bpm = stageNotes[_stageMusicIndex].bpm;
        _noteStates = new List<NoteTriggerState>();
        for (int i = 0; i < stageNotes[_stageMusicIndex].notes.Count; i++)
        {
            _noteStates.Add(NoteTriggerState.None);
        }

        //음악을 재생한다
        if (!IsPlaying)
        {
            InvokeOnMusicStart();
            _musicInstance.start();
            _musicInstance.getTimelinePosition(out int ms);
            MusicStartTime = Time.time - (ms / 1000f);
            IsPlaying = true;
        }

    }
    public float GetCurrentMusicTime()
    {
        if (!_musicInstance.isValid()) return 0f;
        _musicInstance.getTimelinePosition(out int ms);
        CurrentTimelineTime = ms / 1000f;
        return CurrentTimelineTime;
    }
    private void OnDestroy()
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _musicInstance.setCallback(null);
            _musicInstance.release();
        }

        if (_timelineHandle.IsAllocated)
            _timelineHandle.Free();
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

                float beatTime = beat.position / 1000f;
                Instance.CurrentTimelineTime = beatTime;
                InvokeOnBeat(beatTime); // 정확한 beat 시간 전달
                break;

            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                var marker = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                InvokeOnMarkerHit(marker.name);
                break;
        }

        return FMOD.RESULT.OK;
    }
}
