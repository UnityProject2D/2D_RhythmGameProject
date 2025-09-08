using System.Collections.Generic;
using UnityEngine;

public class SubBossSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int poolSize = 30;

    private List<GameObject> subBossPool = new List<GameObject>();
    private int inactiveCount = 0;

    private void Awake()
    {
        InitializePool();
    }

    private void Start()
    {
        SpawnSubBosses();
        RhythmEvents.OnMusicStopped += BossDieJdg;
    }

    private void OnEnable()
    {
        RhythmEvents.OnNote += OnNoteReceived;
        RhythmEvents.OnInputJudged += OnInputJudged;

    }

    private void OnDisable()
    {
        RhythmEvents.OnNote -= OnNoteReceived;
        RhythmEvents.OnInputJudged -= OnInputJudged;
        RhythmEvents.OnMusicStopped -= BossDieJdg;
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

                // 페이드인 효과
                subBoss.GetComponent<SubBossController>().FadeInSubBoss(0.25f);
            }
        }
    }

    public void NotifySubBossDeactivated(GameObject subBoss)
    {
        subBoss.SetActive(false);

        // 여기서 타입 랜덤 변경 처리
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject randomPrefab = enemyPrefabs[randomIndex];

        // Sprite 갈아끼우기
        var oldSR = subBoss.GetComponent<SpriteRenderer>();
        var newSR = randomPrefab.GetComponent<SpriteRenderer>();

        if (oldSR != null && newSR != null)
            oldSR.sprite = newSR.sprite;

        // Animator 갈아끼우기
        var oldAnim = subBoss.GetComponent<Animator>();
        var newAnim = randomPrefab.GetComponent<Animator>();

        if (oldAnim != null && newAnim != null)
            oldAnim.runtimeAnimatorController = newAnim.runtimeAnimatorController;

        inactiveCount++;

        if (inactiveCount >= spawnPoints.Length)
        {
            Invoke(nameof(SpawnSubBosses), 0.2f);
            inactiveCount = 0;
        }
    }

    /*
        subBoss.SetActive(false);
        inactiveCount++;
        if (inactiveCount >= spawnPoints.Length)
        {
            Invoke(nameof(SpawnSubBosses), 0.2f);
        }
     */

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject subBoss = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
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

    private void OnNoteReceived(NoteData note)
    {
        foreach (var subBoss in subBossPool)
        {
            if (subBoss.activeInHierarchy)
            {
                subBoss.GetComponent<SubBossController>().Walk();
            }
        }
    }
    private void OnInputJudged(JudgedContext result)
    {
        if (result.Result <= JudgementResult.Good)
        {
            foreach (var subBoss in subBossPool)
            {
                if (subBoss.activeInHierarchy)
                {
                    subBoss.GetComponent<SubBossController>().HurtAndDeactivate();
                }
            }
        }
    }

    // 서브 보스 죽는 애니메이션
    private void BossDieJdg()
    {
        if (ScoreManager.Instance.Score >= 1000) //10000
        {
            foreach (var subBoss in subBossPool)
            {
                if (subBoss.activeInHierarchy)
                {
                    //RuntimeManager.PlayOneShot("event:/SFX/EnemyDie");
                    subBoss.GetComponent<SubBossController>().Die();
                }
            }
        }
    }
}
