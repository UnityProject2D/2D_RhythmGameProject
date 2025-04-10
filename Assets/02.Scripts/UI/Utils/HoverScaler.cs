using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("마우스 호버 설정")]
    [SerializeField] private float _hoverScale = 1.02f;
    [SerializeField] private float _hoverDuration = 0.2f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsButtonInteractable()) return;
        transform.DOScale(_hoverScale, _hoverDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, _hoverDuration).SetEase(Ease.OutBack);
    }

    private bool IsButtonInteractable()
    {
        Button button = GetComponent<Button>();
        return button != null && button.interactable;
    }
}