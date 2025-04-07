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
    public bool test;
    private Animator _animator;
    public GameObject[] EnemyShadowPrefabs; // (0/W): Jump, (1/S): Down, (2/A): Roll, (3/D): BackFlip
    public Transform[] EnemyShadowSpawnPoint; // 적 그림자 생성 위치
    private GameObject[] enemyShadowPool = new GameObject[4]; // 적 그림자 풀

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 미리 잔상 오브젝트를 생성해서 비활성화 (오브젝트 풀 초기화)
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

    // 미리보기 비트 타이밍마다 호출
    private void OnNotePreviewReceived(NoteData beatNote)
    {
        int index = GetIndexFromKey(beatNote.expectedKey); // 입력 키(WASD) → 인덱스로 변환 (0~3)
        if (index < 0 || index >= enemyShadowPool.Length) return;

        if(test)
        _animator.SetTrigger("Attack");
        GameObject shadow = enemyShadowPool[index]; // 잔상 오브젝트를 해당 위치에 배치하고 활성화
        shadow.transform.position = EnemyShadowSpawnPoint[index].position;
        shadow.SetActive(true);

        //EnemyPatternBuffer.Instance.EnqueuePattern(index); // 잔상 패턴 저장해서 실제 공격 때 사용해야 함. 
        StartCoroutine(HidePatternAfterDelay(shadow, 1f)); // 1f초 후 비활성화
    }

    // 키 문자열 → 인덱스로 변환 (매핑용)
    private int GetIndexFromKey(string key)
    {
        return key switch
        {
            "W" => 0,
            "S" => 1,
            "A" => 2,
            "D" => 3,
            _ => -1
        };
    }

    // 잔상 오브젝트를 일정 시간 후 비활성화
    private IEnumerator HidePatternAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
