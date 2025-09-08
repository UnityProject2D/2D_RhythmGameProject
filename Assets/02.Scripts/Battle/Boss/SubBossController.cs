using DG.Tweening;
using UnityEngine;

public class SubBossController : MonoBehaviour
{
    private Animator _animator;
    private SubBossSpawner _spawner;
    private SpriteRenderer _spriteRenderer;

    public void SetSpawner(SubBossSpawner spawner)
    {
        _spawner = spawner;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Walk()
    {
        _animator.SetTrigger("Attack");
    }

    public void HurtAndDeactivate()
    {
        _animator.SetTrigger("Hurt");
        Invoke(nameof(Deactivate), 0.1f);
    }

    public void Die()
    {
        _animator.SetTrigger("Die");

        // Hurt 애니메이션 시간 끝날 때쯤 페이드아웃 시작 (예: 0.5초 뒤)
        float delay = 1f;
        float fadeDuration = 1f;

        // DOTween 활용해서 알파값 0으로
        _spriteRenderer.DOFade(0, fadeDuration)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                // 페이드아웃 끝나면 비활성화
                gameObject.SetActive(false);
            });
    }

    private void Deactivate()
    {
        _spawner.NotifySubBossDeactivated(gameObject);
    }

    public void FadeInSubBoss(float duration)
    {
        Color c = _spriteRenderer.color;
        c.a = 0;
        _spriteRenderer.color = c;

        _spriteRenderer.DOFade(1f, duration);
    }
}
