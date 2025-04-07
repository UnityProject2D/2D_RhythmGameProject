using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int PlayerMaxHealth = 16;
    private int PlayerCurrentHealth;
    public TextMeshProUGUI PlayerHealthText; // 체력 UI 텍스트

    private Animator _animator;

    private void Awake()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
        _animator = GetComponent<Animator>();
        UpdatePlayerHealthUI();
    }

    public void TakeDamage(int amount)
    {
        PlayerCurrentHealth -= amount;
        Debug.Log($"플레이어 피격! 현재 체력: {PlayerCurrentHealth}");
        //_animator.SetInteger("Direction", (int)RhythmAction.Hit);

        UpdatePlayerHealthUI();

        if (PlayerCurrentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdatePlayerHealthUI()
    {
        if (PlayerHealthText != null)
        {
            PlayerHealthText.text = $"HP: {PlayerCurrentHealth} / {PlayerMaxHealth}";
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        _animator.SetInteger("Direction", (int)RhythmAction.Die);
    }
}
