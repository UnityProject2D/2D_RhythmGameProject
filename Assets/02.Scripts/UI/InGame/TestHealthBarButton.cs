using UnityEngine;

public class TestHealthBarButton : MonoBehaviour
{
    [SerializeField] private DelayedHealthBar _healthBar;

    [SerializeField] private float _damageAmount = 5f;

    public void OnClick()
    {
        _healthBar.CurrentHealth -= _damageAmount;
    }
}
