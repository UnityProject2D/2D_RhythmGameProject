using DG.Tweening;
using UnityEngine;
using static RhythmEvents;

public class SubBossController : MonoBehaviour
{
    private Animator _animator;
    private SubBossSpawner _spawner;
    private SpriteRenderer _spriteRenderer; // 서브보스몹 페이드인&아웃용


    public void SetSpawner(SubBossSpawner spawner)
    {
        _spawner = spawner;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // 서브보스몹 페이드인&아웃용
    }

    private void OnEnable()
    {
        ScoreManager.Instance.OnComboChanged += OnComboChanged;
        OnNote += OnNoteReceived;
        FadeInSubBoss(0.25f); // 서브보스 페이드인
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnComboChanged -= OnComboChanged;
        OnNote -= OnNoteReceived;
    }

    //private void Update()
    //{
    //    _animator.Play("Idle");
    //}

    private void OnComboChanged(int combo)
    {
        if (combo > 0)
        {
            _animator.SetTrigger("Hurt");
            Invoke(nameof(Respawn), 0.1f);
        }
    }

    private void Respawn()
    {
        gameObject.SetActive(false);
        if (_spawner != null)
        {
            _spawner.NotifySubBossDeactivated();
            //FadeInSubBoss(0.5f);
        }
        else
        {
            Debug.Log("Spawner가 할당되지 않음.");
        }
    }

    private void OnNoteReceived(NoteData beatNote)
    {
        _animator.SetTrigger("Walk");
    }

    public void FadeInSubBoss(float duration)
    {
        Color c = _spriteRenderer.color;
        c.a = 0;
        _spriteRenderer.color = c;

        _spriteRenderer.DOFade(1f, duration);
    }
}
