using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryInsertImage : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;
    [SerializeField] private float _fadeDuration = 0.5f; // 페이드 인/아웃 기본 시간

    private void Awake()
    {
        // 초기 설정
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f; // 처음에는 보이지 않음
        }
        else
        {
            Debug.LogError("CanvasGroup component is not assigned in StoryInsert.");
        }

        // 이미지 컴포넌트 확인
        if (_image == null)
        {
            Debug.LogError("Image component is not assigned in StoryInsert.");
        }
    }

    // 이미지 설정
    public void SetImage(Sprite sprite)
    {
        if (_image != null)
        {
            _image.sprite = sprite;
        }
        else
        {
            Debug.LogError("Cannot set image: Image component is null.");
        }
    }

    // 페이드 인 효과
    public IEnumerator FadeIn()
    {
        if (_canvasGroup != null)
        {
            // 페이드 인 시작 전 설정
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(true);

            // 페이드 인 애니메이션
            float elapsedTime = 0f;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeDuration);
                yield return null;
            }

            // 완전히 표시
            _canvasGroup.alpha = 1f;
        }
        else
        {
            Debug.LogError("Cannot fade in: CanvasGroup component is null.");
            yield break;
        }
    }

    // 페이드 아웃 효과
    public IEnumerator FadeOut()
    {
        if (_canvasGroup != null)
        {
            // 페이드 아웃 애니메이션
            float elapsedTime = 0f;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);
                yield return null;
            }

            // 완전히 숨김
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Cannot fade out: CanvasGroup component is null.");
            yield break;
        }
    }

    // 사용자 지정 지속 시간으로 페이드 인
    public IEnumerator FadeIn(float duration)
    {
        float originalDuration = _fadeDuration;
        _fadeDuration = duration;
        yield return StartCoroutine(FadeIn());
        _fadeDuration = originalDuration;
    }

    // 사용자 지정 지속 시간으로 페이드 아웃
    public IEnumerator FadeOut(float duration)
    {
        float originalDuration = _fadeDuration;
        _fadeDuration = duration;
        yield return StartCoroutine(FadeOut());
        _fadeDuration = originalDuration;
    }

    // 즉시 표시
    public void ShowImmediately()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
        }
    }

    // 즉시 숨김
    public void HideImmediately()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
}
