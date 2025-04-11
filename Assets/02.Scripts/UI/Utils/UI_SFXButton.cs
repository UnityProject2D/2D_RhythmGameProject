using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SFXButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("사운드 설정")]
    [SerializeField] private EventReference _clickSound;
    [SerializeField] private EventReference _hoverSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsButtonInteractable()) return;
        RuntimeManager.PlayOneShot(_clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsButtonInteractable()) return;
        RuntimeManager.PlayOneShot(_hoverSound);
    }

    private bool IsButtonInteractable()
    {
        Button button = GetComponent<Button>();
        return button != null && button.interactable;
    }
}
