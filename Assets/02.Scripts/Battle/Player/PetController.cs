using System.Collections.Generic;
using UnityEngine;
using static RhythmEvents;

public class PetController : MonoBehaviour
{
    // 목표: 플레이어 옆에서 플레이어 대신 적을 공격하는 펫 오브젝트
    private Animator _animator;
    public GameObject PetBulletPrefab; // 펫 총알 프리팹
    public Transform PetBulletSpawnPoint; // 펫 총알 발사 위치
    private List<GameObject> PetBulletPool = new(); // 펫 총알 오브젝트 풀
    public Transform[] targetPositions; // 총알이 날아갈 목표 위치들

    private int poolSize = 20; // 총알 풀 사이즈

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 총알 오브젝트 풀 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(PetBulletPrefab);
            bullet.SetActive(false);
            PetBulletPool.Add(bullet);
        }
    }
    private void OnEnable()
    {
        OnInputJudged += OnInputJudgedReceived; // 리듬 입력 판정 이벤트 구독
    }
    private void OnDisable()
    {
        OnInputJudged -= OnInputJudgedReceived; // 리듬 입력 판정 이벤트 구독 해제
    }

    private void OnInputJudgedReceived(JudgedContext result)
    {
        if (result.Result <= JudgementResult.Good)
        {
            FireBullet(); // 퍼펙트 판정일 때만 총알 발사
        }
    }

    private void FireBullet()
    {
        foreach (Transform target in targetPositions)
        {
            GameObject petBullet = GetPetBulletFromPool();
            if (petBullet == null) return;

            petBullet.transform.position = PetBulletSpawnPoint.position;
            petBullet.transform.rotation = Quaternion.identity;

            Vector2 dir = (target.position - PetBulletSpawnPoint.position).normalized;
            petBullet.GetComponent<PetBullet>().direction = dir;

            petBullet.SetActive(true);
        }
    }

    // 사용 가능한 총알을 풀에서 찾아 반환
    private GameObject GetPetBulletFromPool()
    {
        foreach (var petBullet in PetBulletPool)
        {
            if (!petBullet.activeInHierarchy)
                return petBullet;
        }
        return null;
    }
}
