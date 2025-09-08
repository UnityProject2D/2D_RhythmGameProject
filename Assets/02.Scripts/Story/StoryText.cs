using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class StoryText : MonoBehaviour
{
    [SerializeField] private MMF_Player _feedbackPlayer;

    private MMF_TMPTextReveal _textReveal;
    private bool _isRevealing = false;

    private void Awake()
    {
        _textReveal = _feedbackPlayer.GetFeedbackOfType<MMF_TMPTextReveal>();
        if (_textReveal == null)
        {
            Debug.LogError("MMF_TMPTextReveal component not found on this GameObject.");
        }
    }

    public IEnumerator SetTextCoroutine(string text, StoryEffectDataSO typingEffect)
    {
        if (_textReveal != null)
        {
            _isRevealing = true;
            _textReveal.NewText = text;
            if (typingEffect != null)
            {
                _textReveal.RevealDuration = typingEffect.duration;
            }
            // PlayFeedbacksCoroutine은 피드백이 완료될 때까지 대기
            yield return _feedbackPlayer.PlayFeedbacksCoroutine(this.transform.position);
            _isRevealing = false;
        }
        else
        {
            Debug.LogError("MMF_TMPTextReveal component is not assigned.");
            yield break;
        }
    }

    public IEnumerator ResetText()
    {
        if (_textReveal != null)
        {
            // 텍스트 리빌 중지
            _feedbackPlayer.StopFeedbacks();

            _textReveal.NewText = string.Empty;

            // 모든 텍스트 즉시 표시
            _textReveal.TargetTMPText.maxVisibleCharacters = 0;
            _textReveal.TargetTMPText.ForceMeshUpdate();

            // 텍스트 리빌 상태 초기화
            _isRevealing = false;
        }
        else
        {
            Debug.LogError("MMF_TMPTextReveal component is not assigned.");
        }
        yield return null;
    }

    // 텍스트 리빌 즉시 완료 메서드
    public bool CompleteTextRevealIfInProgress()
    {
        if (_isRevealing && _textReveal != null && _textReveal.TargetTMPText != null)
        {
            // 피드백 중지
            _feedbackPlayer.StopFeedbacks();

            // 모든 텍스트 즉시 표시
            _textReveal.TargetTMPText.maxVisibleCharacters = _textReveal.TargetTMPText.textInfo.characterCount;
            _textReveal.TargetTMPText.ForceMeshUpdate();

            _isRevealing = false;
            return true; // 텍스트 리빌이 완료되었음
        }
        return false; // 텍스트 리빌이 진행 중이 아니었음
    }

    // 현재 텍스트 리빌 중인지 확인하는 속성
    public bool IsRevealing => _isRevealing;
}
