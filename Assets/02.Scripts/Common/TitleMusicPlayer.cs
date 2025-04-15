using FMOD.Studio;
using FMODUnity;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
public class TitleMusicPlayer : MonoBehaviour
{
    [SerializeField] private EventReference titleMusic;
    public CustomMMAudioAnalyzer mmaudioAnalyzer;
    private EventInstance _musicInstance;
    private GCHandle _timelineHandle;
    private Queue<Action> _eventQueue = new Queue<Action>();
    private object _lock = new object();
    private bool _isPlaying = false;
    private FMOD.DSP _fftDSP;
    private float[] _fmodSpectrum;
    const int FFT_WINDOW_SIZE = 1024;
    private void Update()
    {
        if (!_isPlaying) return;
        lock (_lock)
        {
            while (_eventQueue.Count > 0)
            {
                _eventQueue.Dequeue()?.Invoke();
            }
        }
        if (_fftDSP.hasHandle())
        {
            IntPtr dataPtr;
            uint length;
            var result = _fftDSP.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out dataPtr, out length);
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
                    if (mmaudioAnalyzer != null)
                    {
                        mmaudioAnalyzer.ExternalSpectrumData = _fmodSpectrum;
                    }
                }
            }
        }
    }
    public void Play()
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _musicInstance.release();
        }
        if (_timelineHandle.IsAllocated)
        {
            _timelineHandle.Free();
        }
        TimelineInfo info = new TimelineInfo();
        _timelineHandle = GCHandle.Alloc(this);
        _musicInstance = RuntimeManager.CreateInstance(titleMusic);
        _musicInstance.setUserData(GCHandle.ToIntPtr(_timelineHandle));
        _musicInstance.setCallback(FMODCallback,
            EVENT_CALLBACK_TYPE.TIMELINE_BEAT |
            EVENT_CALLBACK_TYPE.TIMELINE_MARKER |
            EVENT_CALLBACK_TYPE.STOPPED);
        SetupFmodFFT();
        _musicInstance.start();
        _isPlaying = true;
    }
    private void SetupFmodFFT()
    {
        Debug.Log("[FMOD] FFT 설정 시작");
        RuntimeManager.StudioSystem.flushCommands();
        var resultDSP = RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out _fftDSP);
        Debug.Log($"[FMOD] createDSP 결과: {resultDSP}");
        if (resultDSP == FMOD.RESULT.OK)
        {
            _fftDSP.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
            _fftDSP.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, FFT_WINDOW_SIZE * 2);
            var bus = RuntimeManager.GetBus("bus:/");
            if (bus.hasHandle())
            {
                var resultGroup = bus.getChannelGroup(out FMOD.ChannelGroup channelGroup);
                if (resultGroup == FMOD.RESULT.OK)
                {
                    var resultAdd = channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, _fftDSP);
                    Debug.Log($"[FMOD] addDSP 결과: {resultAdd}");
                }
                else
                {
                    Debug.LogWarning("[FMOD] 채널 그룹 가져오기 실패");
                }
            }
            else
            {
                Debug.LogWarning("[FMOD] bus 핸들 없음");
            }
        }
        else
        {
            Debug.LogWarning("[FMOD] FFT DSP 생성 실패");
        }
    }
    private void OnDisable()
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _musicInstance.setCallback(null);
            _musicInstance.release();
        }
        if (_timelineHandle.IsAllocated)
        {
            _timelineHandle.Free();
        }
    }
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
        var self = handle.Target as TitleMusicPlayer;
        if (self == null) return FMOD.RESULT.OK;
        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                var beat = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                float beatTime = beat.position / 1000f;
                lock (self._lock)
                {
                    self._eventQueue.Enqueue(() => RhythmEvents.SafeInvokeOnBeat(beatTime));
                }
                break;
        }
        return FMOD.RESULT.OK;
    }
}