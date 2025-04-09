using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class FeedbackButton : MonoBehaviour
{
    [Header("마우스 호버 설정")]
    [SerializeField] private float _hoverScale = 1.05f;
    [SerializeField] private float _hoverDuration = 0.2f;

    public void OnPointerEnterHandler(BaseEventData data)
    {
        transform.DOScale(_hoverScale, _hoverDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExitHandler(BaseEventData data)
    {
        transform.DOScale(1f, _hoverDuration).SetEase(Ease.OutBack);
    }
}