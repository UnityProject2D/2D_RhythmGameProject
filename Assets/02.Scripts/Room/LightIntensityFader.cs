using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightIntensityFader : MonoBehaviour {
    //{
    //    [Header(" Light 설정")]
    //    public Light2D TargetLight;

    //    [Header(" Light Intensity 설정")]
    //    public float minIntensity = 0.2f;
    //    public float maxIntensity = 0.2f;
    //    public float duration = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    public MMF_Player MMF_Player;
    private void Start()
    {
        MMF_Player?.PlayFeedbacks();
        //StartCoroutine(FadeLoop());
    }


    //private IEnumerator FadeLoop()
    //{
    //    while(true)
    //    {
    //        // 밝아지기
    //        yield return StartCoroutine(FadeIntensity(minIntensity, maxIntensity, duration));
    //        // 어두워지기
    //        yield return StartCoroutine(FadeIntensity(maxIntensity, minIntensity, duration));
    //    }
    //}
    //private IEnumerator FadeIntensity(float start, float end, float time)
    //{
    //    float elapsed = 0.0f;
    //    while (elapsed < time)
    //    {
    //        float t = elapsed / time;
    //        TargetLight.intensity = Mathf.Lerp(start, end, t);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }
    //    TargetLight.intensity = end;
    //}
}

