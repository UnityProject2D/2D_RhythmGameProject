using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using static RhythmEvents;

public class EnemyController : MonoBehaviour
{
    public bool test;
    private Animator _animator;

    public Transform gunPoint; // 총구 위치 기준 Transform (필수!)

    public Sprite[] directionSprites = new Sprite[4]; // W, S, A, D 순서
    private const int PoolSizePerDirection = 12;
    private List<GameObject>[] shadowPools = new List<GameObject>[4];

    private Transform _playerTransform;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Debug.Log(_animator);
        for (int i = 0; i < 4; i++)
            shadowPools[i] = new List<GameObject>();
    }

    private void Instance_PlayerRegistered()
    {
        _playerTransform = GameManager.Instance.Player.Transform;

        Debug.Log($"EnemyController: PlayerRegistered - {_playerTransform}");
    }

    private void Start()
    {
        if (GameManager.Instance.Player.Controller != null)
        {
            Instance_PlayerRegistered();
        }
        else
        {
            Debug.LogWarning("EnemyController: 플레이어 없네요 - 구독");
            GameManager.Instance.PlayerRegistered += Instance_PlayerRegistered;
        }

        //SetPlayer().Forget();
        OnMusicStopped += EnemyDieJdg;
        for (int dir = 0; dir < 4; dir++)
        {
            for (int i = 0; i < PoolSizePerDirection; i++)
            {
                GameObject shadow = CreateShadowObject(dir);
                shadow.SetActive(false);
                shadowPools[dir].Add(shadow);
            }
        }
    }
    //private async UniTaskVoid SetPlayer()
    //{
    //    while(GameManager.Instance.Player.Transform == null)
    //    {
    //        await UniTask.Yield();
    //    }

    //    _playerTransform = GameManager.Instance.Player.Transform;
    //}
    private void OnEnable()
    {
        OnNotePreview += OnNotePreviewReceived;
    }

    private void OnDisable()
    {
        OnNotePreview -= OnNotePreviewReceived;
        OnMusicStopped -= EnemyDieJdg;
    }

    private void OnNotePreviewReceived(NoteData beatNote)
    {
        PlayAttackSound();

        int dir = GetIndexFromKey(beatNote.expectedKey);
        if (dir < 0 || dir >= 4) return;

        // 애니메이터가 null인지 확인
        if (_animator == null)
        {
            Debug.LogError("Animator is not assigned or missing!");
        }

        // 애니메이션 트리거 설정
        _animator.SetTrigger("Attack");

        GameObject shadow = GetInactiveShadow(dir);
        if (shadow == null) return;

        // 총구 위치 기준 생성
        shadow.transform.position = gunPoint.position;

        // 방향 계산: 총구 → 플레이어
        Vector3 dirToPlayer = (GetTargetPositionFromKey(dir) - gunPoint.position).normalized;
        float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        shadow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 초기 스케일 설정
        shadow.transform.localScale = new Vector3(0.1f, 0.04f, 0.1f);
        shadow.SetActive(true);

        // 투명도 초기화
        var sr = shadow.GetComponent<SpriteRenderer>();
        sr.DOFade(1f, 0f);

        // Y방향 축소로 사라지게
        shadow.transform.DOScaleY(0f, 0.4f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            shadow.SetActive(false);
        });
    }

    private Vector3 GetTargetPositionFromKey(int dir)
    {
        if (_playerTransform == null)
            return gunPoint.position;

        return dir switch
        {
            0 => _playerTransform.position + Vector3.down * 0.25f,     // W - 머리
            1 => _playerTransform.position + Vector3.up * 2f,   // S - 다리
            2 => gunPoint.position + Vector3.left * 1f,   // A - 왼쪽 몸통
            3 => gunPoint.position + Vector3.left * 1f,  // D - 오른쪽 몸통
            _ => _playerTransform.position
        };
    }

    public void PlayAttackSound()
    {
        RuntimeManager.PlayOneShot("event:/SFX/PreviewSound");
    }

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

    private GameObject GetInactiveShadow(int dir)
    {
        foreach (var shadow in shadowPools[dir])
        {
            if (!shadow.activeInHierarchy)
                return shadow;
        }

        return null;
    }

    private GameObject CreateShadowObject(int dir)
    {
        GameObject obj = new GameObject($"Shadow_{dir}");
        obj.transform.SetParent(transform);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = directionSprites[dir];
        sr.sortingLayerName = "Enemy";
        sr.sortingOrder = 10;
        sr.color = GetColor(dir);
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.DOFade(0f, 0f); // DOTween용 초기 투명도 설정

        return obj;
    }

    private Color GetColor(int dir)
    {
        return dir switch
        {
            0 => new Color(1f, 0f, 0f, 0.3f),
            1 => new Color(1f, 0f, 0f, 0.3f),
            2 => new Color(0f, 1f, 1f, 0.3f),
            3 => new Color(0.5f, 0f, 1f, 0.3f),
            _ => Color.white
        };
    }

    ///////// 리듬 시스템 노트 완벽하게 최적화한 후 score 점수 레벨 디자인 진행할 것
    private void EnemyDieJdg()
    {
        if (ScoreManager.Instance.Score >= 10000)
        {
            Debug.Log("적 잔상 죽이기");
            gameObject.SetActive(false);
        }
    }
}
