using System.Collections;
using UnityEngine;
using static RhythmEvents;

public enum EnemyState
{
    Idle,
    Shoot1,
    Shoot2,
    Shoot3,
    Shoot4
}

public class EnemyController : MonoBehaviour
{
    private Animator _animator;
    public GameObject[] EnemyShadowPrefabs; // 0: Jump, 1: Down, 2: Roll, 3: BackFlip
    public Transform[] EnemyShadowSpawnPoint; // 적 그림자 생성 위치
    private GameObject[] enemyShadowPool = new GameObject[4];

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            enemyShadowPool[i] = Instantiate(EnemyShadowPrefabs[i]);
            enemyShadowPool[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        OnNotePreview += OnNotePreviewReceived;
    }

    private void OnDisable()
    {
        OnNotePreview -= OnNotePreviewReceived;
    }

    // 랜덤 패턴 생성 + 잔상 보여주기 + 공격 예약
    private void OnNotePreviewReceived(NoteData beatNote)
    {
        Debug.Log("테스트중입니다!!!!!");
        int r = Random.Range(0, 4); // 0~3

        // 잔상 활성화
        GameObject shadow = enemyShadowPool[r];
        shadow.transform.position = EnemyShadowSpawnPoint[r].position;
        shadow.SetActive(true);
        StartCoroutine(HidePatternAfterDelay(shadow, 1f));

        // 애니메이션도 출력
        EnemyState direction = (EnemyState)(r + 1); // Shoot1 ~ Shoot4
        _animator.SetInteger("Direction", (int)direction);
        _animator.SetTrigger("Attack");
        StartCoroutine(ResetAnimation());

        // EnemyPatternBuffer에 패턴 저장 (공격용)
        EnemyPatternBuffer.Instance.EnqueuePattern(r);
    }

    private IEnumerator HidePatternAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetInteger("Direction", 0);
    }
}
