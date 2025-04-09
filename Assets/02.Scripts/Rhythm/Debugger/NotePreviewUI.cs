using TMPro;
using UnityEngine;

public class NotePreviewUI : MonoBehaviour
{

    public void Setup(string key)
    {
        GetComponent<TextMeshProUGUI>().text = key;
    }

    private void Update()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x - 1, GetComponent<RectTransform>().anchoredPosition.y);
    }
}
