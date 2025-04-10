using System.Collections.Generic;
using UnityEngine;

public class SubBossSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int poolSize = 30; // 충분히 여유 있게 잡음 (3마리 생성이라 최소 3 이상)

    private List<GameObject> subBossPool = new List<GameObject>();
    private int inactiveCount = 0;

    private void Awake()
    {
        InitializePool();
    }

    private void Start()
    {
        SpawnSubBosses();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject subBoss = Instantiate(enemyPrefabs[randomIndex]);
            subBoss.SetActive(false);
            subBoss.GetComponent<SubBossController>().SetSpawner(this);
            subBossPool.Add(subBoss);
        }
    }

    private GameObject GetInactiveSubBoss()
    {
        foreach (var subBoss in subBossPool)
        {
            if (!subBoss.activeInHierarchy)
                return subBoss;
        }
        Debug.LogWarning("풀에 사용 가능한 서브보스가 없습니다!");
        return null;
    }

    public void SpawnSubBosses()
    {
        inactiveCount = 0;
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject subBoss = GetInactiveSubBoss();
            if (subBoss != null)
            {
                subBoss.transform.position = spawnPoint.position;
                subBoss.SetActive(true);
            }
        }
    }

    // 서브보스가 비활성화될 때마다 호출 (세 마리가 모두 비활성화됐는지 체크)
    public void NotifySubBossDeactivated()
    {
        inactiveCount++;

        if (inactiveCount >= spawnPoints.Length)
        {
            inactiveCount = 0; // 비활성화된 서브보스 수 초기화
            // 세 마리 모두 비활성화된 경우에만 재활성화 호출
            Invoke(nameof(SpawnSubBosses), 0.2f);
        }
    }
}
