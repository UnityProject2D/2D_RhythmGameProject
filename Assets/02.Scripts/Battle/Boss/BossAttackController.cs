using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using static RhythmEvents;

public enum BossAttackState
{
    BossAttack,
    BossHurt,
    BossDie,
    BossFlyUp,
    BossSpecial,
    BossFlyDown,
    BossWalk,
    BossWalkAttack

}

public class BossAttackController : MonoBehaviour
{
    private Animator _animator;

    public GameObject BossBulletPrefab;
    private List<GameObject> BossBulletPool = new();
    public Transform PlayerTransform;
    public Transform GunPosition;

    private int poolSize = 10;
    private bool _isDead = false; //////////// 적 사망 여부

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += BossDieJdg;
        // 총알 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(BossBulletPrefab, GunPosition.position, Quaternion.identity);
            bullet.SetActive(false);
            BossBulletPool.Add(bullet);
        }
    }

    private void OnEnable()
    {
        OnNote += OnNoteReceived;
    }

    private void OnDisable()
    {
        OnNote -= OnNoteReceived;
        ScoreManager.Instance.OnScoreChanged -= BossDieJdg;
    }

    private void OnNoteReceived(NoteData beatTime)
    {
        if (_isDead) return; /////////////// 적이 죽었으면 리턴
        PlayAttackSound();
        int index = GetIndexFromKey(beatTime.expectedKey); // 입력 키(WASD) → 인덱스로 변환 (0~3)
        if (index < 0 || index >= BossBulletPool.Count) return;

        // bullet 메서드
        FireBullet(index);
        _animator.SetTrigger("Attack");
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

        bullet.transform.position = GunPosition.position;
        Vector2 direction;
        if (PlayerTransform == null)
            direction = GunPosition.position;

        switch (directionIndex)
        {
            case 0: direction = (PlayerTransform.position + Vector3.down * 0.25f) - GunPosition.position; break;     // W - 머리
            case 1: direction = (PlayerTransform.position + Vector3.up * 2f) - GunPosition.position; break;   // S - 다리
            case 2: direction = Vector3.left; break;   // A - 왼쪽 몸통
            case 3: direction = Vector3.left; break;  // D - 오른쪽 몸통
            default: direction = PlayerTransform.position; break;
        }

        direction = direction.normalized;

        // 회전 방향 기준으로 이동 방향 설정
        bullet.GetComponent<BossBullet>().direction = direction;
        bullet.SetActive(true);
    }


    private GameObject GetBulletFromPool()
    {
        foreach (var bullet in BossBulletPool)
        {
            if (!bullet.activeInHierarchy)
                return bullet;
        }

        var newBullet = Instantiate(BossBulletPrefab, GunPosition.position, Quaternion.identity);

        newBullet.SetActive(false);
        BossBulletPool.Add(newBullet);
        return newBullet;
    }
    public void PlayAttackSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/AttackSound");
    }


    ///////// 적이 죽으면!! -> ScoreManager StageCleared 코드 완성된 후 점검 후 수정할것!
    ///////// 리듬 시스템 노트 완벽하게 최적화한 후 score 점수 레벨 디자인 진행할 것
    private void BossDieJdg(int score)
    {
        if (_isDead) return;
        if (score >= 5000)
        {
            _isDead = true;
            _animator.SetTrigger("Die");
        }
    }
}
