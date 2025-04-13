using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;
using static RhythmEvents;

public class DummyController : MonoBehaviour
{
    private Animator _animator;

    public GameObject EnemyBulletPrefab;
    private List<GameObject> EnemyBulletPool = new();
    public Transform PlayerTransform;
    public Transform GunPosition;

    private int poolSize = 10;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnMusicStopped += EnemyDieJdg;
        // 총알 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(EnemyBulletPrefab, GunPosition.position, Quaternion.identity);
            bullet.SetActive(false);
            EnemyBulletPool.Add(bullet);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        OnNote += OnNoteReceived;
    }

    private void OnDisable()
    {
        OnNote -= OnNoteReceived;
        OnMusicStopped -= EnemyDieJdg;
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

    public void PlayAttackSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/AttackSound");
    }

    private void EnemyDieJdg()
    {
        if (ScoreManager.Instance.Score >= 10000)
        {
            RuntimeManager.PlayOneShot("event:/SFX/EnemyDie");
            _animator.SetTrigger("Die");
        }
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

    private void FireBullet(int directionIndex)
    {
        GameObject bullet = GetBulletFromPool();
        if (bullet == null) return;

        bullet.transform.position = GunPosition.position;
        Vector2 direction;
        if (PlayerTransform == null)
            direction = GunPosition.position;
        else
        {
            switch (directionIndex)
            {
                case 0: direction = (PlayerTransform.position + Vector3.down * 0.25f) - GunPosition.position; break;     // W - 머리
                case 1: direction = (PlayerTransform.position + Vector3.up * 2f) - GunPosition.position; break;   // S - 다리
                case 2: direction = (PlayerTransform.position + Vector3.up * 1f) - GunPosition.position; break;  // A - 왼쪽 몸통
                case 3: direction = (PlayerTransform.position + Vector3.up * 0.5f) - GunPosition.position; break; // D - 오른쪽 몸통
                default: direction = PlayerTransform.position; break;
            }
        }
        direction = direction.normalized;

        // 회전 방향 기준으로 이동 방향 설정
        bullet.GetComponent<EnemyBullet>().direction = direction;
        bullet.SetActive(true);
    }
}
