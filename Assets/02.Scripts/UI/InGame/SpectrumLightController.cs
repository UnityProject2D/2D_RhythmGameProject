using UnityEngine;
using UnityEngine.Rendering.Universal;
public class SpectrumLightController : MonoBehaviour
{
    [Header("설정")]
    public TitleMusicPlayer musicPlayer;
    [Tooltip("여기에 조명 8개를 넣으세요")]public Light2D[] lights = new Light2D[8];

    [Header("조명 설정")]
    [Tooltip("초기 밝기")] public float lightIntensity = 0.2f;
    [Tooltip("최소 밝기")]public float minIntensity = 0.2f;
    [Tooltip("최대 밝기")] public float maxIntensity = 2.5f;
    [Tooltip("밝기 변하는 속도")]public float lerpSpeed = 5f;


    private float[] _levels;
    private void Start()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = lightIntensity;
        }
    }
    private void Update()
    {
        if (musicPlayer == null || musicPlayer.mmaudioAnalyzer == null) return;
        _levels = musicPlayer.mmaudioAnalyzer.NormalizedBufferedBandLevels;
        if (_levels == null || _levels.Length < lights.Length) return;
        for (int i = 0; i < lights.Length; i++)
        {
            float level = _levels[i];
            if (float.IsNaN(level)) continue;
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, level);
            lights[i].intensity = Mathf.Lerp(lights[i].intensity, targetIntensity, Time.deltaTime * lerpSpeed);
        }
    }
}