using UnityEngine;

public class PetBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10f;
    public float lifetime = 7f;

    private void OnEnable()
    {
        Invoke(nameof(Disable), lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
}
