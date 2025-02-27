using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveDirection = Vector2.up; // Start moving upward
    private bool canTurn = true;

    void Update()
    {
        // Handle movement
        transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * speed * Time.deltaTime;

        // Handle input for turning
        if (canTurn)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TurnLeft();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TurnRight();
            }
        }
    }

    void TurnLeft()
    {
        // Rotate the sprite
        transform.Rotate(0, 0, 90);

        // Update movement direction
        Vector2 newDir = new Vector2(-moveDirection.y, moveDirection.x);
        moveDirection = newDir;

        // Prevent multiple turns in the same frame
        StartCoroutine(TurnCooldown());
    }

    void TurnRight()
    {
        // Rotate the sprite
        transform.Rotate(0, 0, -90);

        // Update movement direction
        Vector2 newDir = new Vector2(moveDirection.y, -moveDirection.x);
        moveDirection = newDir;

        // Prevent multiple turns in the same frame
        StartCoroutine(TurnCooldown());
    }

    IEnumerator TurnCooldown()
    {
        canTurn = false;
        yield return new WaitForSeconds(0.1f);
        canTurn = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with: " + other.gameObject.name + " Tag: " + other.gameObject.tag);

        // Handle collisions with walls
        if (other.CompareTag("Wall"))
        {
            // Stop movement
            speed = 0;
            enabled = false;
            Debug.Log("Game Over! Hit a wall.");
        }
    }
}
