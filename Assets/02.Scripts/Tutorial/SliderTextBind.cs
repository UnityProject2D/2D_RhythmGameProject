using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextBinder : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI ValueText;

    private void Awake()
    {
        UpdateText(); // 초기값 설정
    }

    public void UpdateText()
    {
        ValueText.text = Mathf.RoundToInt(Slider.value).ToString(); // 정수로 표시
    }
}