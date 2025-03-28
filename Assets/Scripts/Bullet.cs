using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // rb.velocity = transform.up * speed; // Uncomment and set direction properly if needed
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet hit: " + other.gameObject.name + " with tag: " + other.tag);
        if (other.CompareTag("Player") || other.CompareTag("Op"))
        {
            PlayerMovement cycle = other.GetComponent<PlayerMovement>(); // Use the correct script name attached to your cycle
            if (cycle != null)
            {
                cycle.CrashByBullet(); // Call a method to handle the crash
            }
            Destroy(gameObject);
            // Optional: Destroy bullet if it hits walls or trails
        if (other.CompareTag("Wall") || other.CompareTag("Trail") || other.CompareTag("OpponentBorder"))
        {
            Destroy(gameObject);
        }
    
        }
    }

}
