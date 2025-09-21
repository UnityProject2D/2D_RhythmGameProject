using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerGroundAnimController : MonoBehaviour
{
    [SerializeField] 
    private GroundLockMove _groundLock;
    [SerializeField]
    private string _moveParam = "MoveSpeed";
    [SerializeField]
    private string _groundedParam = "Grounded";
    [SerializeField]
    private float _speedScale = 1.0f;
    [SerializeField]
    private float _damp = 0.1f;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        bool grounded = _groundLock && _groundLock.IsGrounded;
        anim.SetBool(_groundedParam, grounded);

        // 지면 속도 사용
        float groundSpeed = _groundLock ? _groundLock.GroundSpeedXAbs : 0f; // 지형이 움직이는형

        // 그라운드 위일 때만 이동
        float speed = grounded ? groundSpeed * _speedScale : 0f;

        // 감쇠 적용
        anim.SetFloat(_moveParam, speed, _damp, Time.deltaTime);
    }
}
