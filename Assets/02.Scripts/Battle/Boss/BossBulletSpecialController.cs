using System.Collections;
using UnityEngine;

public class BossBulletSpecialController : MonoBehaviour
{
    private Animator _animator;
    private Vector3 _playerTransform;
    public float _missMoveSpeed = 20f;
    public bool isExploded = false;
    public Vector3 Destination;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        StartCoroutine(Check());
    }
    public IEnumerator Check()
    {
        yield return new WaitForSeconds(3.5f);
        if (!isMoving)
        {
            isMoving = false;
            isExploded = false;
            gameObject.SetActive(false);
        }
    }
    public void Setup(Vector2 spawnPos, Transform player)
    {
        transform.position = spawnPos;
        _playerTransform = new Vector3(-6.3f, -1f);
        isExploded = false;
        isMoving = false;
    }

    // Good/Perfect 판정 시 호출: 폭발 애니메이션 실행 후 Invoke로 DisableBullet() 호출
    //public void ExplodeToBoss()
    //{
    //    if (isExploded)
    //        return;
    //    isExploded = true;
    //    _animator.SetTrigger("Explode");
    //    Invoke(nameof(DisableBullet), 0.3f);
    //}

    // Miss 판정 시 호출
    public void ExPlodeToPlayer()
    {
        if (_playerTransform == null)
        {
            Debug.LogError("플레이어 트랜스폼 할당 안됨!");
            return;
        }
        if (isExploded)
            return;
    }
    public void StartMove()
    {
        StartCoroutine(MoveToPlayerRoutine());
        isMoving = true;
    }
    bool isMoving;
    private IEnumerator MoveToPlayerRoutine()
    {
        while (Vector2.Distance(transform.position, _playerTransform) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _playerTransform, _missMoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _playerTransform; // 정확한 위치로 설정

        if (!isExploded)
        {
            isExploded = true;
            _animator.SetTrigger("Explode");
            // DisableBullet()는 코루틴이 아닌 애니메이션 이벤트를 통해 호출하도록 합니다.
            yield break;
        }

        while (Vector2.Distance(transform.position, Destination) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, Destination, _missMoveSpeed * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
        isExploded = false;
        isMoving = false;
    }

    // 이 메서드는 폭발 애니메이션 클립에 Animation Event로 등록하세요.
    public void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}
