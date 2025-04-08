using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    //public PlayerHealth PlayerHealth; // 플레이어 체력 스크립트
    public GameObject PlayerHpBar; // 플레이어 부모 체력바
    public Image PlayerHealthFill; // 플레이어 체력바 필

    //private void OnEnable()
    //{
    //    if (PlayerHealth.Instance != null) // 플레이어가 죽은 경우 이벤트 구독하지 않음
    //    PlayerHealth.Instance.OnPlayerHealthChanged += HandleHealthChanged; // 플레이어 체력 변경 이벤트 구독
    //}
    private void Start()
    {
        if (PlayerHealth.Instance != null) // 플레이어가 죽은 경우 이벤트 구독하지 않음
            PlayerHealth.Instance.OnPlayerHealthChanged += HandleHealthChanged; // 플레이어 체력 변경 이벤트 구독
    }

    private void OnDisable()
    {
        if (PlayerHealth.Instance != null) // 플레이어가 죽은 경우 이벤트 구독하지 않음
            PlayerHealth.Instance.OnPlayerHealthChanged -= HandleHealthChanged; // 플레이어 체력 변경 이벤트 구독 해제
    }

    private void HandleHealthChanged(float currentHealth)
    {
        float maxHealth = PlayerHealth.Instance.PlayerMaxHealth;
        float normalized = currentHealth / maxHealth;

        if (PlayerHealthFill != null)
        {
            PlayerHealthFill.fillAmount = normalized; // 체력바 필 양 조절
        }

        if (PlayerHpBar != null)
        {
            PlayerHpBar.SetActive(true); // 체력바 활성화
        }
    }
}
