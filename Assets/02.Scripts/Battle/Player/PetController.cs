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

    private int poolSize = 16; // 총알 풀 사이즈

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

    private void Update()
    {
        GameObject petBullet = GetPetBulletFromPool(); // 총알 오브젝트를 해당 위치에 배치하고 활성화
        if (petBullet == null) return; // 비활성화된 총알이 없으면 리턴
        petBullet.transform.position = PetBulletSpawnPoint.position;
        petBullet.transform.rotation = Quaternion.Euler(0f, 0f, 90f); // 총알 회전 각도 설정 (Z축 회전)
        petBullet.GetComponent<PetBullet>().direction = Vector2.right; // 총알 방향 설정
        petBullet.SetActive(true);
    }

    private void OnInputJudgedReceived(JudgedContext result)
    {
        if (result.Result == JudgementResult.Perfect)
        {
            FireBullet(); // 퍼펙트 판정일 때만 총알 발사
        }
    }

    private void FireBullet()
    {
        GameObject petBullet = GetPetBulletFromPool(); // 총알 오브젝트를 해당 위치에 배치하고 활성화
        if (petBullet == null) return; // 비활성화된 총알이 없으면 리턴
        petBullet.transform.position = PetBulletSpawnPoint.position;
        petBullet.transform.rotation = Quaternion.Euler(0f, 0f, 45f); // 총알 회전 각도 설정 (Z축 회전)
        petBullet.GetComponent<PetBullet>().direction = Vector2.down; // 총알 방향 설정
        petBullet.SetActive(true);
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
