using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 20f;
    public float lifetime = 7f;

    private void OnEnable()
    {
        Invoke(nameof(Disable), lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        Debug.Log("플레이어에게 피격!");
    //        //collision.GetComponent<PlayerHealth>()?.TakeDamage(1);
    //        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
    //        if (playerHealth != null)
    //        {
    //            playerHealth.TakeDamage(1); // 여기서 1은 데미지 값이야
    //        }
    //        gameObject.SetActive(false);
    //    }
    //}
}
