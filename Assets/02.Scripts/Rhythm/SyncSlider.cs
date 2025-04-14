using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SyncSlider : MonoBehaviour
{
    public Slider audioOffsetSlider;
    public TextMeshProUGUI audioOffsetValueText;
    public Slider videoOffsetSlider;
    public TextMeshProUGUI videoOffsetValueText;
    void Start()
    {
        audioOffsetSlider.value = SyncSettings.InputOffsetMs;
        audioOffsetSlider.onValueChanged.AddListener((val) =>
        {
            SyncSettings.InputOffsetMs = val;
            audioOffsetValueText.text = $"{val:F0}ms";
        });
        videoOffsetSlider.value = SyncSettings.VideoOffsetMs;
        videoOffsetSlider.onValueChanged.AddListener((val) =>
        {
            SyncSettings.VideoOffsetMs = val;
            videoOffsetValueText.text = $"{val:F0}ms";
        });
    }

}
