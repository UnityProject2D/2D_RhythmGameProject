using System.Collections.Generic;
using UnityEngine;

public class BossAttackSpecialController : MonoBehaviour
{
    public Transform bossTop;
    public Transform player;
    public GameObject bulletPrefab;
    public int bulletCount = 8;
    public float radius = 2f;
    public float offsetX = 0f;
    public float offsetY = -1f;
    public float rotateOffset = 0f;

    private List<BossBulletSpecial> bulletPool = new();
    private int bulletIndex = 0;

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
        SpawnBullet();
    }

    private void OnNoteReceived(NoteData note)
    {
        ShootBullet();
    }

    private void OnInputJudged(JudgedContext result)
    {
        if (result.Result >= JudgementResult.Good)
        {
            Debug.Log("스페셜");
            foreach (var b in bulletPool)
            {
                if (b.gameObject.activeInHierarchy && b.State == BossBulletSpecial.BulletState.Moving)
                {
                    b.Explode();
                    break;
                }
            }
        }
    }

    private void SpawnBullet()
    {
        BossBulletSpecial bullet = GetBulletFromPool();

        float angle = (360f / bulletCount) * bulletIndex + rotateOffset;
        Vector2 offset = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * radius;

        Vector2 spawnPos = (Vector2)bossTop.position + offset + new Vector2(offsetX, offsetY);
        bullet.Setup(spawnPos);
        bullet.gameObject.SetActive(true);

        bulletIndex++;
    }

    private void ShootBullet()
    {
        foreach (var b in bulletPool)
        {
            if (b.gameObject.activeInHierarchy && b.State == BossBulletSpecial.BulletState.Idle)
            {
                b.Shoot(player.position);
                break;
            }
        }
    }

    private BossBulletSpecial GetBulletFromPool()
    {
        foreach (var b in bulletPool)
        {
            if (!b.gameObject.activeInHierarchy)
                return b;
        }

        GameObject obj = Instantiate(bulletPrefab);
        BossBulletSpecial bullet = obj.GetComponent<BossBulletSpecial>();
        bulletPool.Add(bullet);
        return bullet;
    }
}