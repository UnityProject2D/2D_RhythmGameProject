using UnityEngine;

public class BossBulletSpecial : MonoBehaviour
{
    public enum BulletState { Idle, Moving, Exploding }

    public BulletState State { get; private set; } = BulletState.Idle;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 controlPoint;

    private float t = 0f;
    public float moveTime = 0.25f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (State)
        {
            case BulletState.Idle:
                break;

            case BulletState.Moving:
                MoveBullet();
                break;

            case BulletState.Exploding:
                // 애니메이션 대기 중
                break;
        }
    }

    // 총알 위치 지정 (스폰 직후)
    public void Setup(Vector2 spawnPos)
    {
        transform.position = spawnPos;
        State = BulletState.Idle;
    }

    // 총알 발사 (베지어 이동 시작)
    public void Shoot(Vector2 targetPos)
    {
        startPoint = transform.position;
        endPoint = targetPos;
        controlPoint = (startPoint + endPoint) * 0.5f + Vector2.up * 3f;

        t = 0f;
        State = BulletState.Moving;
    }

    // 총알 이동 처리
    private void MoveBullet()
    {
        t += Time.deltaTime / moveTime;

        if (t >= 1f)
        {
            Explode();
            return;
        }

        Vector2 pos = Mathf.Pow(1 - t, 2) * startPoint +
                      2 * (1 - t) * t * controlPoint +
                      Mathf.Pow(t, 2) * endPoint;

        transform.position = pos;
    }

    // 폭발 처리 (애니메이션만 실행)
    public void Explode()
    {
        if (State == BulletState.Exploding) return;

        State = BulletState.Exploding;
        _animator.SetTrigger("Explode");
    }

    // 애니메이션 Event 로 호출될 비활성화
    public void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}
