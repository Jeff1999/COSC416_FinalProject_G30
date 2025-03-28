// Bullet.cs
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // rb.linearVelocity = transform.up * speed; // Use .up if your firepoint is facing up
    }

//     void OnTriggerEnter2D(Collider2D other)
// {
    
//  Debug.Log("Bullet hit: " + other.gameObject.name + " with tag: " + other.tag);
//     if (other.gameObject.name == "AIOpponent" || other.gameObject.name == "Player2"|| other.gameObject.name == "Player")
// {
//     Debug.Log("Fuck me:"); 
   
//     Destroy(other.gameObject);
//     Destroy(gameObject);

// }


// }

void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet hit: " + other.gameObject.name + " with tag: " + other.tag);

        if ( other.gameObject.name == "Player2"|| other.gameObject.name == "Player")
        {
            TwoPlayerMovements cycle = other.GetComponent<TwoPlayerMovements>();

            if (cycle != null)
            {
                cycle.CrashByBullet(); // Call the crash logic
            }
            else
            {
                Debug.LogWarning("2PlayerMovement not found on: " + other.gameObject.name);
            }

            Destroy(gameObject); // Destroy the bullet
        }
        if (other.gameObject.name == "AIOpponent")
        {
            AIController cycle = other.GetComponent<AIController>();

            if (cycle != null)
            {
                cycle.CrashByBullet(); // Call the crash logic
            }
            else
            {
                Debug.LogWarning("PlayerMovement not found on: " + other.gameObject.name);
            }

            Destroy(gameObject); // Destroy the bullet
        }

        // 💥 Destroy trail if bullet hits it
    if (other.CompareTag("Trail"))
    {
        Debug.LogWarning("TrailTouched");


        Destroy(other.gameObject); // Destroy trail segment
        Destroy(gameObject);       // Destroy bullet
    }

    if (other.CompareTag("Wall") || other.CompareTag("OpponentBorder"))
    {
        Destroy(gameObject);
    }


        if (other.CompareTag("Wall") || other.CompareTag("Trail") || other.CompareTag("OpponentBorder"))
        {
            Destroy(gameObject);
        }
    }
}
