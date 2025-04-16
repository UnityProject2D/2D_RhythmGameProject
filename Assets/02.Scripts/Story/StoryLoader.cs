using System.Collections;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLoader : MonoBehaviour
{
    [SerializeField] private StorySceneDataSO _scene;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private StoryDialogue _dialogue;
    [SerializeField] private MMF_Player _feedbackPlayer;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private StoryInsertImage _insertImage;
    [SerializeField] private GameObject Title;
    [SerializeField] private string _sceneName;
    private void Start()
    {
        if (_scene != null)
        {
            StartCoroutine(PlayScene());
        }
        else
        {
            Debug.LogError("Story scene data is not assigned.");
        }
    }

    private IEnumerator PlayScene()
    {
        SetBackground(_scene.background);

        foreach (var dialogue in _scene.dialogues)
        {
            yield return StartCoroutine(ResetEffects());
            yield return StartCoroutine(_dialogue.SetDialogueCoroutine(dialogue));
            yield return new WaitForSeconds(dialogue.delayAfter);
        }

        // 씬 종료
        Debug.Log("Scene finished.");
        GameSceneManager.Instance.ChangeScene(_sceneName);
    }

    private IEnumerator ResetEffects()
    {
        yield return StartCoroutine(_dialogue.ResetDialogue());
        if (_canvasGroup.alpha != 0f)
        {
            // 1초 동안 페이드인 효과
            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime);
                yield return null;
            }
        }
    }

    private void SetBackground(Sprite background)
    {
        if (_backgroundImage != null)
        {
            _backgroundImage.sprite = background;
        }
        else
        {
            Debug.LogError("Background image is not assigned.");
        }
    }

    public IEnumerator ChangeBackground(Sprite background, float duration)
    {
        if (_backgroundImage != null)
        {

            yield return StartCoroutine(ExecuteFadeOut(duration/2));
            _backgroundImage.sprite = background;

            yield return StartCoroutine(ExecuteFadeIn(duration/2));
        }
        else
        {
            Debug.LogError("Background image is not assigned.");
        }
    }

    public void ExcuteGlitch(float duration)
    {
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.SetArtifacts(true);
        }
        else
        {
            Debug.LogError("VFX Manager is not assigned.");
        }
    }

    public void ShowTitle()
    {
        Title.SetActive(true); 
    }

    // 페이드아웃 효과를 위한 공개 메서드
    public IEnumerator ExecuteFadeOut(float duration)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }
        else
        {
            Debug.LogError("CanvasGroup component is not properly set up.");
            yield return new WaitForSeconds(duration); // 에러가 있어도 시간은 대기
        }
    }
    public IEnumerator ExecuteFadeIn(float duration)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("CanvasGroup component is not properly set up.");
            yield return new WaitForSeconds(duration); // 에러가 있어도 시간은 대기
        }
    }

    public IEnumerator ExecuteInsertImage(Sprite sprite, float duration)
    {
        if (_insertImage != null)
        {
            _insertImage.SetImage(sprite);
            yield return StartCoroutine(_insertImage.FadeIn());
            yield return new WaitForSeconds(duration);
            yield return StartCoroutine(_insertImage.FadeOut());
        }
        else
        {
            Debug.LogError("StoryInsert component is not assigned.");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameSceneManager.Instance.ChangeScene(_sceneName);
        }
    }
}
