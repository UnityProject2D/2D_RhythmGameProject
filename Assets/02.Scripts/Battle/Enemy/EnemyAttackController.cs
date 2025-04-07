using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RhythmEvents;

public enum EnemyAttackState
{
    Idle,
    Shoot1,
    Shoot2,
    Shoot3,
    Shoot4
}

public class EnemyAttackController : MonoBehaviour
{
    private Animator _animator;

    public GameObject EnemyBulletPrefab;
    public Transform EnemyBulletSpawnPoints;
    private List<GameObject> EnemyBulletPool = new();

    private int poolSize = 10;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 총알 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(EnemyBulletPrefab);
            bullet.SetActive(false);
            EnemyBulletPool.Add(bullet);
        }
    }

    private void OnEnable()
    {
        OnNote += OnNoteReceived;
    }

    private void OnDisable()
    {
        OnNote -= OnNoteReceived;
    }

    private void OnNoteReceived(NoteData beatTime)
    {
        int index = GetIndexFromKey(beatTime.expectedKey); // 입력 키(WASD) → 인덱스로 변환 (0~3)
        if (index < 0 || index >= EnemyBulletPool.Count) return;

        // bullet 메서드
        FireBullet(index);

        _animator.SetInteger("Direction", index + 1);
        _animator.SetTrigger("Attack");


        StartCoroutine(ResetAnimation());
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

    private void FireBullet(int directionIndex)
    {
        GameObject bullet = GetBulletFromPool();
        if (bullet == null) return;

        bullet.transform.position = EnemyBulletSpawnPoints.position;

        // 방향별 회전 각도 수동 설정 (Z축 회전)
        float zRotation = directionIndex switch
        {
            0 => -2.226f,    // 위
            1 => 0f,  // 아래
            2 => 3.079f,   // 왼쪽
            3 => 7.893f,  // 오른쪽
            _ => 0f
        };
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);

        // 회전 방향 기준으로 이동 방향 설정
        bullet.GetComponent<EnemyBullet>().direction = Vector2.left;
        bullet.SetActive(true);
    }

    private GameObject GetBulletFromPool()
    {
        foreach (var bullet in EnemyBulletPool)
        {
            if (!bullet.activeInHierarchy)
                return bullet;
        }
        return null;
    }

    private IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetInteger("Direction", 0);
    }
}
