using System.Collections.Generic;
using UnityEngine;

public class BossBulletSpecialSpawner : MonoBehaviour
{
    [Header("참조")]
    public Transform bossTop;
    public Transform player;
    public GameObject bulletPrefab;

    [Header("총알 원형 생성")]
    public int bulletCount = 8;
    public float radius = 2f;
    public float offsetX = 0f;
    public float offsetY = -1f;
    public float rotateOffset = 0f;

    public BossAttackController attackController;

    private readonly List<BossBulletSpecialController> bulletPool = new();
    private int bulletIndex = 0;
    private readonly Queue<BossBulletSpecialController> bulletQueue = new();

    private void OnEnable()
    {
        RhythmEvents.OnNotePreview += OnNotePreviewReceived;
        RhythmEvents.OnNote += OnNoteReceived;
        RhythmEvents.OnInputJudged += OnInputJudged;
    }

    private void OnDisable()
    {
        RhythmEvents.OnNotePreview -= OnNotePreviewReceived;
        RhythmEvents.OnNote -= OnNoteReceived;
        RhythmEvents.OnInputJudged -= OnInputJudged;
    }

    private void OnNotePreviewReceived(NoteData beatNote)
    {
        if (attackController.page != 1) return;
        SpawnBullet();
    }

    private void OnNoteReceived(NoteData note)
    {
        if (attackController.page != 1) return;
        if (bulletQueue.TryPeek(out var bullet))
        {
            bullet.StartMove();
        }
    }

    private void OnInputJudged(JudgedContext result)
    {
        if (attackController.page != 1) return;
        if (result.Result <= JudgementResult.Good)
        {
            if (bulletQueue.TryDequeue(out var bullet))
            {
                bullet.isExploded = true;
            }
            else
            {
                Debug.LogError("ExplodeBoss: Dequeue 실패 - 큐에 총알 없음");
            }

        }
        else if (result.Result == JudgementResult.Miss || result.Result == JudgementResult.Bad)
        {
            if (bulletQueue.TryDequeue(out var bullet))
            {
                bullet.ExPlodeToPlayer();
            }
            else
            {
                Debug.LogError("ExplodePlayer: Dequeue 실패 - 큐에 총알 없음");
            }
        }

    }

    private void SpawnBullet()
    {
        BossBulletSpecialController bullet = GetBulletFromPool();

        // 각 총알이 원형으로 배치도록 계산 (임의의 각도 offset)
        float angle = (360f / bulletCount) * bulletIndex + rotateOffset;
        Vector2 offset = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * radius;
        Vector2 spawnPos = (Vector2)bossTop.position + offset + new Vector2(offsetX, offsetY);

        bullet.Setup(spawnPos, player);
        bullet.gameObject.SetActive(true);

        // 총알이 활성화되면 Queue에 추가합니다.
        bulletQueue.Enqueue(bullet);
        
        bulletIndex++;
    }

    private BossBulletSpecialController GetBulletFromPool()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.gameObject.activeInHierarchy)
                return bullet;
        }
        GameObject obj = Instantiate(bulletPrefab);
        BossBulletSpecialController bulletController = obj.GetComponent<BossBulletSpecialController>();
        bulletPool.Add(bulletController);
        return bulletController;
    }

}