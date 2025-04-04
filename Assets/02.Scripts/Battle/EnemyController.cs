using System.Collections;
using UnityEngine;
using static RhythmEvents;

public enum EnemyState
{
    Idle,
    Shoot1,
    Shoot2,
    //Shoot3,
    //Shoot4
}

public class EnemyController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        OnNotePreview += OnBeat; // float 파라미터 있음!
    }

    private void OnDisable()
    {
        OnNotePreview -= OnBeat;
    }


    private void OnBeat(NoteData beatTime)
    {
        Debug.Log($"온비트 출력");
        int r = Random.Range(0, 2); // 0~3까지 랜덤으로 선택. 범위 설정 신중하게.
        EnemyState direction = EnemyState.Idle; // 기본값
        switch (r)
        {
            case 0: direction = EnemyState.Shoot1; break;
            case 1: direction = EnemyState.Shoot2; break;
                //case 2: direction = EnemyState.Shoot3; break;
                //case 3: direction = EnemyState.Shoot4; break;
        }
        _animator.SetInteger("Direction", (int)direction);
        _animator.SetTrigger("Attack");
        StartCoroutine(ResetAnimation()); // 애니메이션 재생
    }
    private IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(0.5f); // 애니메이션 재생 시간
        _animator.SetInteger("Direction", 0); //(int)RhythmAction.None
    }
}
