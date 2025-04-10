using TMPro;
using UnityEngine.Rendering;
using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using DG.Tweening;

public class OverDriveEffect : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform box;
    public TMP_Text overdriveText;
    public TMP_Text overdriveTextPrefab;
    public TMP_Text[] systemTexts; // 순차 출력용
    public Volume glitchVolume;

    [Header("Settings")]
    public float boxAnimationTime;
    public float duration;


    void Start()
    {
        StartCoroutine(PlayOverDriveSequence());
    }

    IEnumerator PlayOverDriveSequence()
    {
        // 1. 중앙 박스 커지기
        yield return AnimateBox(true);

        // 2. 텍스트들 활성화
        StartCoroutine(ShowSystemLogs());

        // 3. 글리치 효과 키기
        VFXManager.Instance.SetArtifacts(true);

            // 4. 30초 대기
        yield return new WaitForSeconds(duration);

        // 5. 텍스트 깜빡이며 사라짐
        StartCoroutine(BlinkOut(overdriveText));
        foreach (var txt in systemTexts)
            StartCoroutine(BlinkOut(txt));

        overdriveTextPrefab.gameObject.SetActive(false);
        overdriveText.gameObject.SetActive(false);
        // 6. 박스 작아지기
        yield return AnimateBox(false);

        // 7. 글리치 꺼짐
        VFXManager.Instance.SetArtifacts(false);
        Destroy(gameObject);
    }

    IEnumerator AnimateBox(bool expand)
    {
        yield return box.DOScale((expand ? Vector3.one : Vector3.zero), boxAnimationTime).SetEase(Ease.OutCubic).WaitForCompletion();
    }

    IEnumerator BlinkText(TMP_Text txt, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            txt.enabled = !txt.enabled;
            yield return new WaitForSeconds(0.25f);
            time += 0.25f;
        }

        txt.enabled = true;
    }

    IEnumerator BlinkOut(TMP_Text txt)
    {
        for (int i = 0; i < 10; i++)
        {
            txt.enabled = !txt.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        txt.enabled = false;
    }

    IEnumerator ShowSystemLogs()
    {
        foreach (var txt in systemTexts)
        {
            txt.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        for (int i=0; i<systemTexts.Length; i++)
        {
            for(int j=0; j < i; j++)
            {
                systemTexts[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, systemTexts[j].GetComponent<RectTransform>().anchoredPosition.y+30f);
            }
            systemTexts[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(Random.value/1.5f);
        }

        overdriveTextPrefab.gameObject.SetActive(true);
        overdriveText.gameObject.SetActive(true);
        StartCoroutine(BlinkText(overdriveText,duration));
        overdriveTextPrefab.GetComponent<MMF_Player>().PlayFeedbacks();
    }
}
