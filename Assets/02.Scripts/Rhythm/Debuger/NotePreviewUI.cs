using UnityEngine;
using TMPro;

public class NotePreviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyText;

    public void Setup(string key)
    {
        keyText.text = key;
    }
}
