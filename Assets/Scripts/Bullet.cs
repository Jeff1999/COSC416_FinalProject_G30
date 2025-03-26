// Bullet.cs
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed; // Use `.up` if your firepoint is facing up
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject != this.gameObject)
        {
            Destroy(other.gameObject); // Destroy the other player
            Destroy(gameObject);       // Destroy the bullet
        }
        else if (other.CompareTag("Boundary") || other.CompareTag("Wall") || other.CompareTag("OpponentBorder"))
        {
            Destroy(gameObject); // Bullet hits boundary
        }
    }
}
