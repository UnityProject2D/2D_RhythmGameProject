using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Game : MonoBehaviour
{
    // 화면에 전체 시간 표시
    public TextMeshProUGUI TotalTimer;

    private int seconds = 0;

    private void Start()
    {
        StartCoroutine(ShowTimer());
    }

    private IEnumerator ShowTimer()
    {
        while (true)
        {
            seconds++;
            TotalTimer.text = $"Timer: {seconds}";
            yield return new WaitForSeconds(1f);
        }
    }
}
