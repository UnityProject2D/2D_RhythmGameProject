using UnityEngine;

public class SubBossSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;

    private void Start()
    {
        SpawnSubBosses();
    }

    private void OnEnable()
    {
        RhythmEvents.OnInputJudged += OnInpugJudged;

    }
    private void OnDisable()
    {
        RhythmEvents.OnInputJudged -= OnInpugJudged;
    }

    private void SpawnSubBosses()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomIndex], spawnPoints[i].position, Quaternion.identity);
        }
    }

    private void OnInpugJudged(JudgedContext result)
    {
        if (result.Result == JudgementResult.Perfect)
        {
            // 적 3마리 모두 Hurt 애니메이션과 함께 비활성화 3초후 재생성
        }
    }


}
