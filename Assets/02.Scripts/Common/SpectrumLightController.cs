using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpectrumLightController : MonoBehaviour
{
    [Header("설정")]
    public TitleMusicPlayer musicPlayer;
    public Light2D[] lights = new Light2D[8];
    public float minIntensity = 0.2f;
    public float maxIntensity = 2.5f;
    public float lerpSpeed = 5f;

    private float[] _levels;

    private void Start()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = 2f;
        }
    }
    private void Update()
    {
        if (musicPlayer == null || musicPlayer.mmaudioAnalyzer == null) return;

        _levels = musicPlayer.mmaudioAnalyzer.NormalizedBufferedBandLevels;
        if (_levels == null || _levels.Length < lights.Length) return;

        for (int i = 0; i < lights.Length; i++)
        {
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, _levels[i]);
            lights[i].intensity = Mathf.Lerp(lights[i].intensity, targetIntensity, Time.deltaTime * lerpSpeed);
        }
    }
}