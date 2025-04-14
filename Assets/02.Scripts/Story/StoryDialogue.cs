using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

// EventTrigger를 사용하기 위해 필요한 이벤트 인터페이스 구현
public class StoryDialogue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private StoryText _text;
    [SerializeField] private bool _skipOnClick = true;
    [SerializeField] private StoryLoader _storyLoader; // StoryLoader 참조 추가

    private StoryDialogueDataSO _dialogueData;
    private bool _isWaitingAfterDialogue = false;
    private bool _skipWaitRequested = false;

    private void Start()
    {
        // StoryLoader가 할당되지 않았다면 부모나 부모의 부모에서 찾기
        if (_storyLoader == null)
        {
            _storyLoader = GetComponentInParent<StoryLoader>();
            if (_storyLoader == null)
            {
                Debug.LogWarning("StoryLoader not found. Some effects might not work properly.");
            }
        }
    }

    public IEnumerator SetDialogueCoroutine(StoryDialogueDataSO dialogueData)
    {
        _dialogueData = dialogueData;
        SetPortrait(_dialogueData.character.portrait);
        SetNameText(_dialogueData.character.displayName);

        // 원본 리스트를 복사하여 작업 (수정 시 원본 데이터 보존)
        var effects = new List<StoryEffectDataSO>(_dialogueData.effects);

        // effects 중에 타이핑 효과를 꺼내서 저장하기
        StoryEffectDataSO typingEffect = null;
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            if (effects[i].type == StoryEffectType.Typing)
            {
                typingEffect = effects[i];
                effects.RemoveAt(i);
                break;
            }
        }

        // 텍스트 표시가 완료될 때까지 대기
        yield return StartCoroutine(SetTextCoroutine(_dialogueData.text, typingEffect));

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

        // 대기 시간이 끝나거나 스킵된 후에 남은 효과들 실행
        if (effects.Count > 0)
        {
            yield return StartCoroutine(ExecuteRemainingEffects(effects));
        }
    }

    public IEnumerator ResetDialogue()
    {
        // 대화 내용 초기화
        if (_text != null)
        {
            yield return _text.ResetText();
        }
        else
        {
            Debug.LogError("StoryText component is not assigned.");
        }

        // 대기 상태 초기화
        _isWaitingAfterDialogue = false;
        _skipWaitRequested = false;

        // 포트레이트와 이름 텍스트 초기화
        SetPortrait(null);
        SetNameText(string.Empty);
    }

    // 남은 효과들을 순차적으로 실행하는 코루틴
    private IEnumerator ExecuteRemainingEffects(List<StoryEffectDataSO> effects)
    {
        foreach (var effect in effects)
        {
            // 효과 유형에 따라 다른 처리
            switch (effect.type)
            {
                case StoryEffectType.FadeOut:
                    // 화면 흔들기 효과 실행
                    yield return StartCoroutine(ExecuteFadeOutEffect(effect));
                    break;

                case StoryEffectType.InsertImage:
                    // 화면 번쩍임 효과 실행
                    yield return StartCoroutine(ExecuteInsertImageEffect(effect));
                    break;

                // 필요한 다른 효과 유형 추가
                default:
                    Debug.LogWarning($"Unhandled effect type: {effect.type}");
                    break;
            }
        }
    }

    // FadeOut 효과를 실행하는 코루틴
    private IEnumerator ExecuteFadeOutEffect(StoryEffectDataSO effect)
    {
        if (_storyLoader != null)
        {
            // StoryLoader의 캔버스 그룹을 사용하여 페이드아웃 실행
            yield return StartCoroutine(_storyLoader.ExecuteFadeOut(effect.duration));
        }
        else
        {
            Debug.LogError("StoryLoader reference is missing. Cannot execute FadeOut effect.");
            yield return new WaitForSeconds(effect.duration);
        }
    }

    // InsertImage 효과를 실행하는 코루틴
    private IEnumerator ExecuteInsertImageEffect(StoryEffectDataSO effect)
    {
        if (_storyLoader != null)
        {
            // StoryLoader의 이미지 삽입 효과 실행
            yield return StartCoroutine(_storyLoader.ExecuteInsertImage(effect.image, effect.duration));
        }
        else
        {
            Debug.LogError("StoryLoader reference is missing. Cannot execute InsertImage effect.");
            yield return new WaitForSeconds(effect.duration);
        }
    }

    private void SetPortrait(Sprite sprite)
    {
        if (_portrait != null)
        {
            _portrait.sprite = sprite;
            _portrait.color = sprite != null ? Color.white : new Color(1, 1, 1, 0); // 투명도 조절
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
            _nameText.text = name.Length > 0 ? $"[{name}]" : string.Empty;
        }
        else
        {
            Debug.LogError("Name text is not assigned.");
        }
    }

    private IEnumerator SetTextCoroutine(string text, StoryEffectDataSO typingEffect)
    {
        if (_text != null)
        {
            yield return _text.SetTextCoroutine(text, typingEffect);
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
