using UnityEngine;
using TMPro;

public class NotePreviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyText;

    public void Setup(string key)
    {
        keyText.text = key;
    }

    private void Update()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x-1, GetComponent<RectTransform>().anchoredPosition.y);
    }
}
