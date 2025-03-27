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

    void OnTriggerEnter2D(Collider2D other)
{
    
 Debug.Log("Bullet hit: " + other.gameObject.name + " with tag: " + other.tag);
    if (other.gameObject.name == "AIOpponent" || other.gameObject.name == "Player2"|| other.gameObject.name == "Player")
{
    Debug.Log("Fuck me:"); 
   
    Destroy(other.gameObject);
    Destroy(gameObject);

}


}
}
