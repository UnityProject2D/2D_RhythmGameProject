using UnityEngine;

public class TestHealthBarButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 상위 Canvas에서 UI_HealthBar 오브젝트를 찾아서 DelayedHealthBar 컴포넌트를 가져옵니다.
        var healthBar = GameObject.Find("UI_HealthBar").GetComponent<DelayedHealthBar>();
        // DelayedHealthBar의 MaxHealth 속성을 100으로 설정합니다.
        healthBar.MaxHealth = 1000f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 버튼 클릭 시 호출되는 메서드입니다.
    public void OnClick()
    {
        // 상위 Canvas에서 UI_HealthBar 오브젝트를 찾아서 DelayedHealthBar 컴포넌트를 가져옵니다.
        var healthBar = GameObject.Find("UI_HealthBar").GetComponent<DelayedHealthBar>();
        // DelayedHealthBar의 CurrentHealth 속성을 50으로 설정합니다.
        healthBar.CurrentHealth -= 50f;
    }
}
