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
    private Transform _playerTransform;
    public Transform[] GunPosition;

    private int poolSize = 10;
    private bool _isDead = false; //////////// 적 사망 여부
    //private bool isFlyingLooping = false; //////////// 비행 애니메이션 루프 여부

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        OnMusicStopped += BossDieJdg;
        OnMarkerHit += JudgeEnd;
        // 총알 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            int r = Random.Range(0, GunPosition.Length);
            GameObject bullet = Instantiate(BossBulletPrefab, GunPosition[r].position, Quaternion.identity);
            bullet.SetActive(false);
            BossBulletPool.Add(bullet);
        }
        if (GameManager.Instance.Player.Controller != null)
        {
            Instance_PlayerRegistered();
        }
        else
        {
            Debug.LogWarning("BossAttackController: 플레이어 없네요 - 구독");
            GameManager.Instance.PlayerRegistered += Instance_PlayerRegistered;
        }
    }
    private void Instance_PlayerRegistered()
    {
        _playerTransform = GameManager.Instance.Player.Transform;

        Debug.Log($"BossAttackController: PlayerRegistered - {_playerTransform}");
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        OnNote += OnNoteReceived;
    }

    private void OnDisable()
    {

        OnMarkerHit -= JudgeEnd;
        OnNote -= OnNoteReceived;
        OnMusicStopped -= BossDieJdg;
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

        int r = Random.Range(0, GunPosition.Length);
        bullet.transform.position = GunPosition[r].position;
        Vector2 direction;
        if (_playerTransform == null)
            direction = GunPosition[r].position;

        switch (directionIndex)
        {
            case 0: direction = (_playerTransform.position + Vector3.down * 0.25f) - GunPosition[r].position; break;     // W - 머리
            case 1: direction = (_playerTransform.position + Vector3.up * 2f) - GunPosition[r].position; break;   // S - 다리
            case 2: direction = Vector3.left; break;   // A - 왼쪽 몸통
            case 3: direction = Vector3.left; break;  // D - 오른쪽 몸통
            default: direction = _playerTransform.position; break;
        }

        direction = direction.normalized;

        // 회전 방향 기준으로 이동 방향 설정
        bullet.GetComponent<BossBullet>().direction = direction;
        bullet.SetActive(true);
    }


    private GameObject GetBulletFromPool()
    {
        int r = Random.Range(0, GunPosition.Length);
        foreach (var bullet in BossBulletPool)
        {
            if (!bullet.activeInHierarchy)
                return bullet;
        }

        var newBullet = Instantiate(BossBulletPrefab, GunPosition[r].position, Quaternion.identity);

        newBullet.SetActive(false);
        BossBulletPool.Add(newBullet);
        return newBullet;
    }
    public void PlayAttackSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/AttackSound");
    }

    private void JudgeEnd(string marker)
    {
        if (marker == "End")
        {
            BossDieJdg();
        }
    }
    ///////// 적이 죽으면!! -> ScoreManager StageCleared 코드 완성된 후 점검 후 수정할것!
    ///////// 리듬 시스템 노트 완벽하게 최적화한 후 score 점수 레벨 디자인 진행할 것
    private void BossDieJdg()
    {
        if (ScoreManager.Instance.Score >= 1000) //10000
        {
            RuntimeManager.PlayOneShot("event:/SFX/EnemyDie");
            _animator.SetTrigger("Die");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pet"))
        {
            _animator.SetTrigger("Hurt");
        }
    }

    //private void Test()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        _animator.SetTrigger("FlyUp");
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        _animator.SetTrigger("Special");
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        _animator.SetTrigger("FlyDown");
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha4))
    //    {
    //        _animator.SetTrigger("Walk");
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha5))
    //    {
    //        _animator.SetTrigger("WalkAttack");
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha6))
    //    {
    //        isFlyingLooping = !isFlyingLooping;
    //        if (isFlyingLooping)
    //        {
    //            StartCoroutine(FlyingLoopRoutine());
    //        }
    //    }
    //}

    //private IEnumerator FlyingLoopRoutine()
    //{
    //    while (isFlyingLooping)
    //    {
    //        _animator.SetTrigger("FlyUp");
    //        yield return new WaitForSeconds(1f);
    //        _animator.SetTrigger("FlyDown");
    //        yield return new WaitForSeconds(1f);
    //    }
    //}
}
