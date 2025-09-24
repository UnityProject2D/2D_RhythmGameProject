using UnityEngine;

public class GroundLockMove : MonoBehaviour
{
    [Header("MoveValue")]
    public LayerMask groundMask;
    public float DownPos = 0.6f; // 발 밑 공간
    public float DeepPos = 25f;  // 탐색 깊이
    public float SideGap = 0.1f; // 좌우 폭
    public float SkinPos = 0.002f; // 위치 조절

    private Rigidbody2D _rigidBody;
    private Collider2D _collider;

    // 현재 밟고 있는 지형 추적
    private Transform _groundTransform;
    private Vector3 _lastGroundPos;
    private bool _isGrounded;

    public bool IsGrounded => _isGrounded; // 애니메이터 읽기
    public float GroundSpeedXAbs { get; private set; }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _rigidBody.freezeRotation = true;
        _rigidBody.bodyType = RigidbodyType2D.Kinematic; // 제자리 설정
        _rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        Physics2D.SyncTransforms();

        FollowGroundDeltaY();

        if (TrySnapBelow(out var hit, DownPos) || TrySnapBelow(out hit, DeepPos))
            StickTo(hit);
        else
        {
            _isGrounded = false;
            _groundTransform = null;
            GroundSpeedXAbs = 0f;
        }
    }

    void FollowGroundDeltaY()
    {
        if (!_isGrounded || _groundTransform == null)
        {
            GroundSpeedXAbs = 0f;
            return;
        }

        Vector3 groundTrans = _groundTransform.position;
        float dy = groundTrans.y - _lastGroundPos.y;
        float dx = groundTrans.x - _lastGroundPos.x;

        GroundSpeedXAbs = Mathf.Abs(dx) / Time.fixedDeltaTime;

        _rigidBody.MovePosition(new Vector2(_rigidBody.position.x, _rigidBody.position.y + dy));
        _lastGroundPos = groundTrans;
    }

    bool TrySnapBelow(out RaycastHit2D hit, float probe)
    {
        Vector2 center = _collider.bounds.center;
        Vector2 size = _collider.bounds.size; size.x += SideGap;
        float dist = _collider.bounds.extents.y + probe + SkinPos;

        hit = Physics2D.CapsuleCast(center, size, CapsuleDirection2D.Vertical,
                                    0f, Vector2.down, dist, groundMask);
        return hit.collider != null;
    }

    void StickTo(RaycastHit2D hit)
    {
        float extY = _collider.bounds.extents.y;
        float targetY = hit.point.y + extY + SkinPos;
        _rigidBody.MovePosition(new Vector2(_rigidBody.position.x, targetY));
        _rigidBody.linearVelocity = Vector2.zero; _rigidBody.angularVelocity = 0f;

        _groundTransform = hit.rigidbody ? hit.rigidbody.transform : hit.collider.transform;
        _lastGroundPos = _groundTransform.position;
        _isGrounded = true;
    }
}
