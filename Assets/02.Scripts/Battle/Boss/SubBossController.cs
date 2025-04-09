using UnityEngine;
using static RhythmEvents;

public class SubBossController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        ScoreManager.Instance.OnComboChanged += OnComboChanged;
    }

    private void OnEnable()
    {
        OnNote += OnNoteReceived;
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
        }
    }

    private void OnNoteReceived(NoteData beatNote)
    {
        _animator.SetTrigger("Walk");
    }
}
