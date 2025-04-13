using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using static RhythmEvents;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Concurrent;

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
    public bool IsRestart = false;
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

    private EventInstance _musicInstance;
    private GCHandle _timelineHandle;
    private int _stageMusicIndex;
    private List<NoteTriggerState> _noteStates;

    private ConcurrentQueue<Action> _eventQueue = new ConcurrentQueue<Action>();
    private object _lock = new object();
    public CustomMMAudioAnalyzer mmaudioAnalyzer;

    public int StageMusicIndex
    {
        get => _stageMusicIndex;
        private set => _stageMusicIndex = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if(PlayerState.Instance!=null)
            PlayerState.Instance.GetComponent<PlayerHealth>().OnPlayerDied += OnPlayerDie;
        else
        {
            if(GameManager.Instance!=null)
                GameManager.Instance.PlayerRegistered += OnRegistered;
        }
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.OnStageDataLoaded += OnLoadedStage;
            Debug.Log($"[RhythmManager] OnStageDataLoaded 등록됨");
        }
    }

    private void OnRegistered()
    {
        PlayerState.Instance.GetComponent<PlayerHealth>().OnPlayerDied += OnPlayerDie;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정
    }

    public void OnLoadedStage(StageData stageData)
    {
        Debug.Log($"[RhythmManager] OnLoadedStage 호출됨, {stageData.StageName}, {stageData.StageIndex}");
        _stageMusicIndex = stageData.StageIndex;
        
        if (IsTest||(_stageMusicIndex > 0 && IsPlaying == false))
        {
            IsPlaying = true;
            Play();
        }
        else
        {
            IsPlaying = false;
        }

        InvokeOnMusicReady();
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle")
        {
            Debug.Log("[RhythmManager] DestroyOnRestart 호출됨");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!IsPlaying) return;
        while (_eventQueue.TryDequeue(out Action action))
        {
            action?.Invoke();
        }

        float offset = SyncSettings.InputOffsetMs / 1000f;
        float currentTime = GetCurrentMusicTime() - offset;

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
                        //Debug.Log($"[노트 발동] 키: {note.expectedKey}, 비트: {note.beat}");
                    }
                    break;
            }
        }


        if (_fftDSP.hasHandle())
        {
            IntPtr dataPtr;
            uint length;
            var result = _fftDSP.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out dataPtr, out length);
            //Debug.Log($"[FMOD] getParameterData result: {result}, hasHandle: {_fftDSP.hasHandle()}");
            if (result == FMOD.RESULT.OK)
            {
                FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(dataPtr, typeof(FMOD.DSP_PARAMETER_FFT));
                if (fftData.numchannels > 0 && fftData.spectrum != null && fftData.length > 0)
                {
                    if (_fmodSpectrum == null || _fmodSpectrum.Length != fftData.length)
                    {
                        _fmodSpectrum = new float[fftData.length];
                    }

                    fftData.getSpectrum(0, ref _fmodSpectrum);

                    //Debug.Log($"FFT[0]: {_fmodSpectrum[0]}");

                    if (mmaudioAnalyzer != null)
                    {
                        mmaudioAnalyzer.ExternalSpectrumData = _fmodSpectrum;
                    }
                }
            }
        }
    }
    public void StopMusic()
    {
        if (_musicInstance.isValid())
        {

            // 콜백을 제거하여 더 이상 콜백이 호출되지 않도록 합니다.
            _musicInstance.setCallback(null);
            // 사용자 데이터를 초기화합니다.
            _musicInstance.setUserData(IntPtr.Zero);


            IsRestart = true;
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.release();
            IsPlaying = false;
        }
        if (_timelineHandle.IsAllocated)
        {
            _timelineHandle.Free();
        }

        MusicStartTime = -1f;
    }
    public void Play()
    {
        //음악을 종료하고
        if (_musicInstance.isValid())
        {
            // 콜백을 제거하여 더 이상 콜백이 호출되지 않도록 합니다.
            _musicInstance.setCallback(null);
            // 사용자 데이터를 초기화합니다.
            _musicInstance.setUserData(IntPtr.Zero);

            IsRestart = true;
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
        _musicInstance.setCallback(FMODCallback,
            EVENT_CALLBACK_TYPE.TIMELINE_BEAT |
            EVENT_CALLBACK_TYPE.TIMELINE_MARKER |
    EVENT_CALLBACK_TYPE.STOPPED);

        _bpm = stageNotes[_stageMusicIndex].bpm;
        _noteStates = new List<NoteTriggerState>();
        for (int i = 0; i < stageNotes[_stageMusicIndex].notes.Count; i++)
        {
            _noteStates.Add(NoteTriggerState.None);
        }
        IsPlaying = false;
        //음악을 재생한다
        if (!IsPlaying)
        {
            InvokeOnMusicStart();

            SetupFmodFFT();
            _musicInstance.start();
            _musicInstance.getTimelinePosition(out int ms);
            MusicStartTime = Time.time - (ms / 1000f);
            IsPlaying = true;
        }

    }

    private FMOD.DSP _fftDSP;
    private float[] _fmodSpectrum;
    const int FFT_WINDOW_SIZE = 1024;

    private void SetupFmodFFT()
    {
        Debug.Log("[FMOD] SetupFmodFFT 시작");
        FMODUnity.RuntimeManager.StudioSystem.flushCommands();

        var resultDSP = FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out _fftDSP);
        Debug.Log($"[FMOD] createDSP result: {resultDSP}");

        if (resultDSP == FMOD.RESULT.OK)
        {
            _fftDSP.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
            _fftDSP.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, FFT_WINDOW_SIZE * 2);

            var bus = FMODUnity.RuntimeManager.GetBus("bus:/BGM");

            if (bus.hasHandle())
            {
                var resultGroup = bus.getChannelGroup(out FMOD.ChannelGroup channelGroup);
                Debug.Log($"[FMOD] getChannelGroup result: {resultGroup}");

                if (resultGroup == FMOD.RESULT.OK)
                {
                    var resultAdd = channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, _fftDSP);
                    Debug.Log($"[FMOD] addDSP result: {resultAdd}");
                }
                else
                {
                    Debug.LogWarning("[FMOD] X getChannelGroup 실패");
                }
            }
            else
            {
                Debug.LogWarning("[FMOD] X bus.hasHandle() == false");
            }
        }
        else
        {
            Debug.LogWarning("[FMOD] X createDSPByType 실패");
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
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.setCallback(null);
            _musicInstance.release();
        }

        if (_timelineHandle.IsAllocated)
            _timelineHandle.Free();
    }
    public void OnPlayerDie()
    {
        IsRestart = false;
        Debug.Log("[RhythmManager] HandleDie() 호출");
        StartCoroutine(FadeOutPitch(_musicInstance,1f));
    }
    public IEnumerator FadeOutPitch(EventInstance musicInstance, float duration)
    {
        float time = 0f;
        float startPitch = 1f;
        float endPitch = 0.2f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float pitch = Mathf.Lerp(startPitch, endPitch, t);
            musicInstance.setPitch(pitch);
            yield return null;
        }

        musicInstance.setPitch(endPitch);

        yield return new WaitForSecondsRealtime(3f);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

        try
        {
            var handle = GCHandle.FromIntPtr(dataPtr);
            if (!handle.IsAllocated || handle.Target == null)
                return FMOD.RESULT.OK;
        }
        catch (ArgumentException ex)
        {
            Debug.LogError("GCHandle 해제 이후 호출됨: " + ex);
            return FMOD.RESULT.OK;
        }
        switch (type)
        {
            case EVENT_CALLBACK_TYPE.STOPPED:
                lock (Instance._lock)
                {
                    Instance._eventQueue.Enqueue(() => RhythmEvents.InvokeOnMusicStopped());
                }
                break;

            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                var marker = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                string markerName = marker.name;
                lock (Instance._lock)
                {
                    Instance._eventQueue.Enqueue(() => RhythmEvents.InvokeOnMarkerHit(markerName));
                }
                break;

            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                var beat = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                float beatTime = beat.position / 1000f;
                lock (Instance._lock)
                {
                    Instance._eventQueue.Enqueue(() => RhythmEvents.InvokeOnBeat(beatTime));
                }
                break;
        }

        return FMOD.RESULT.OK;
    }
}
