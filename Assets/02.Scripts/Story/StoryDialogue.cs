using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// EventTrigger를 사용하기 위해 필요한 이벤트 인터페이스 구현
public class StoryDialogue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private StoryText _text;
    [SerializeField] private bool _skipOnClick = true;

    private StoryDialogueDataSO _dialogueData;
    private bool _isWaitingAfterDialogue = false;
    private bool _skipWaitRequested = false;

    public IEnumerator SetDialogueCoroutine(StoryDialogueDataSO dialogueData)
    {
        _dialogueData = dialogueData;
        SetPortrait(_dialogueData.character.portrait);
        SetNameText(_dialogueData.character.displayName);

        // 텍스트 표시가 완료될 때까지 대기
        yield return StartCoroutine(SetTextCoroutine(_dialogueData.text));

        // 대화 후 대기 시간 동안 대기 (클릭으로 스킵 가능)
        if (_dialogueData.delayAfter > 0)
        {
            _isWaitingAfterDialogue = true;
            _skipWaitRequested = false;

            float timer = 0f;
            while (timer < _dialogueData.delayAfter && !_skipWaitRequested)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            _isWaitingAfterDialogue = false;
        }
    }

    private void SetPortrait(Sprite sprite)
    {
        if (_portrait != null)
        {
            _portrait.sprite = sprite;
        }
        else
        {
            Debug.LogError("Portrait image is not assigned.");
        }
    }

    private void SetNameText(string name)
    {
        if (_nameText != null)
        {
            _nameText.text = $"[{name}]";
        }
        else
        {
            Debug.LogError("Name text is not assigned.");
        }
    }

    private IEnumerator SetTextCoroutine(string text)
    {
        if (_text != null)
        {
            yield return _text.SetTextCoroutine(text);
        }
        else
        {
            Debug.LogError("StoryText component is not assigned.");
            yield break;
        }
    }

    // 텍스트 즉시 완료 또는 대기 스킵 메서드
    public bool CompleteTextRevealIfInProgress()
    {
        // 텍스트 리빌 중이면 텍스트 리빌 완료
        if (_text != null && _text.IsRevealing)
        {
            return _text.CompleteTextRevealIfInProgress();
        }
        // 대기 중이면 대기 스킵
        else if (_isWaitingAfterDialogue)
        {
            _skipWaitRequested = true;
            return true;
        }
        return false;
    }

    // IPointerClickHandler 인터페이스 구현
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_skipOnClick)
        {
            CompleteTextRevealIfInProgress();
        }
    }

    // 현재 텍스트 리빌 중이거나 대기 중인지 확인
    public bool IsRevealing => (_text != null && _text.IsRevealing) || _isWaitingAfterDialogue;
}
