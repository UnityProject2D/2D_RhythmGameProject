using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 20f;
    public float lifetime = 3f;
    public GameObject particle;
    private void OnEnable()
    {
        Invoke(nameof(Disable), lifetime);
        // 회전 (Z축 회전)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
