using Cysharp.Threading.Tasks;
using FMODUnity;
using MoreMountains.Feedbacks;
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
    private List<GameObject> EnemyBulletPool = new();
    private Transform _playerTransform;
    public Transform GunPosition;
    public MMF_Player CoinEffect;
    public GameObject QuantumKey;
    public bool IsDead;
    private int poolSize = 10;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        OnMusicStopped += EnemyDieJdg;

        OnMarkerHit += JudgeEnd;
        // 총알 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(EnemyBulletPrefab, GunPosition.position, Quaternion.identity);
            bullet.SetActive(false);
            EnemyBulletPool.Add(bullet);
        }

        if (GameManager.Instance.Player.Controller != null)
        {
            Instance_PlayerRegistered();
        }
        else
        {
            Debug.LogWarning("EnemyAttackController: 플레이어 없네요 - 구독");
            GameManager.Instance.PlayerRegistered += Instance_PlayerRegistered;
        }

        //SetPlayer().Forget();
    }

    private void Instance_PlayerRegistered()
    {
        _playerTransform = GameManager.Instance.Player.Transform;

        Debug.Log($"{gameObject.name} EnemyAttackController: PlayerRegistered - {_playerTransform}");
    }

    //private async UniTaskVoid SetPlayer()
    //{
    //    while (GameManager.Instance.Player.Transform == null)
    //    {
    //        Debug.Log($"{GameManager.Instance.Player.Transform}");
    //        await UniTask.Yield();
    //    }

    //    _playerTransform = GameManager.Instance.Player.Transform;
    //}


    private void OnEnable()
    {
        OnNote += OnNoteReceived;
    }

    private void OnDisable()
    {
        OnNote -= OnNoteReceived;
        OnMusicStopped -= EnemyDieJdg;

        GameManager.Instance.PlayerRegistered -= Instance_PlayerRegistered;
        OnMarkerHit -= JudgeEnd;
    }

    private void OnNoteReceived(NoteData beatTime)
    {
        PlayAttackSound();
        int index = GetIndexFromKey(beatTime.expectedKey); // 입력 키(WASD) → 인덱스로 변환 (0~3)
        if (index < 0 || index >= EnemyBulletPool.Count) return;

        // bullet 메서드
        FireBullet(index);

        _animator.SetInteger("Direction", index + 1);
        _animator.SetTrigger("Attack");


        //StartCoroutine(ResetAnimation());
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
        if (_playerTransform == null)
            direction = GunPosition.position;
        else
        {
            switch (directionIndex)
            {
                case 0: direction = (_playerTransform.position + Vector3.down * 0.25f) - GunPosition.position; break;     // W - 머리
                case 1: direction = (_playerTransform.position + Vector3.up * 2f) - GunPosition.position; break;   // S - 다리
                case 2: direction = Vector3.left; break;   // A - 왼쪽 몸통
                case 3: direction = Vector3.left; break;  // D - 오른쪽 몸통
                default: direction = _playerTransform.position; break;
            }
        }
        direction = direction.normalized;

        // 회전 방향 기준으로 이동 방향 설정
        bullet.GetComponent<EnemyBullet>().direction = direction;
        bullet.SetActive(true);
    }


    private GameObject GetBulletFromPool()
    {
        foreach (var bullet in EnemyBulletPool)
        {
            if (!bullet.activeInHierarchy)
                return bullet;
        }

        var newBullet = Instantiate(EnemyBulletPrefab, GunPosition.position, Quaternion.identity);

        newBullet.SetActive(false);
        EnemyBulletPool.Add(newBullet);
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
            EnemyDieJdg();
        }
    }
    ///////// 적이 죽으면!! -> ScoreManager StageCleared 코드 완성된 후 점검 후 수정할것!
    ///////// 리듬 시스템 노트 완벽하게 최적화한 후 score 점수 레벨 디자인 진행할 것
    private void EnemyDieJdg()
    {
        if (IsDead) return;
        IsDead = true;
        if (ScoreManager.Instance.Score >= 10000)
        {
            RuntimeManager.PlayOneShot("event:/SFX/EnemyDie");
            _animator.SetTrigger("Die");
            DropKey().Forget();

            async UniTaskVoid DropKey()
            {
                Instantiate(QuantumKey, transform.position, Quaternion.identity);
                CoinEffect?.PlayFeedbacks();
                await UniTask.Delay(1000,false);
                Destroy(gameObject);
            }
        }
    }
}
