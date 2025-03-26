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
    Debug.Log("Bullet hit something: " + other.gameObject.name + " Tag: " + other.tag);

    if (other.CompareTag("Player"))
    {
        Debug.Log("Bullet hit a Player! Destroying...");
        Destroy(other.gameObject);  // or whatever effect you want
        Destroy(gameObject);
    }
    else if (other.CompareTag("Wall") || other.CompareTag("Boundary") || other.CompareTag("OpponentBorder"))
    {
        Debug.Log("Bullet hit a wall/boundary. Destroying bullet.");
        Destroy(gameObject);
    }
}
void Update()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
    foreach (var hit in hits)
    {
        Debug.Log("Detected nearby: " + hit.name + " with tag: " + hit.tag);
    }
}



}
