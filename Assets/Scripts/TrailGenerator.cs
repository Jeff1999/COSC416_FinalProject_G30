using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class WorkingTrailGenerator : MonoBehaviour
{
    public Transform playerTransform;
    public float lineWidth = 0.2f;
    public Color trailColor = Color.cyan;
    public float spawnDistance = 0.5f; // Distance behind player

    // New variables for corner detection
    public float normalPointDistance = 0.1f;    // Normal distance between points
    public float cornerPointDistance = 0.05f;   // Shorter distance at corners

    private LineRenderer lineRenderer;
    private Vector3 lastPosition;
    private bool isFirstFrame = true;
    private Vector3 lastDirection;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player transform not assigned!");
            enabled = false;
            return;
        }
        // Setup line renderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;

        // Initialize position
        lastPosition = playerTransform.position;

        // Get initial direction
        float angle = playerTransform.eulerAngles.z * Mathf.Deg2Rad;
        lastDirection = new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0);

        // Start with 1 point
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, GetPointBehindPlayer());

        // This is the essential part: set the GameObject's tag properly
        gameObject.tag = "Trail";

        // Make sure this isn't a child of the player
        if (transform.IsChildOf(playerTransform))
        {
            transform.SetParent(null);
        }
    }

    void LateUpdate()
    {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        // Get current player direction
        float angle = playerTransform.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 currentDirection = new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0);

        // Check if direction changed (player turned)
        bool isTurning = Vector3.Dot(lastDirection, currentDirection) < 0.99f;

        // Use appropriate distance threshold
        float distanceThreshold = isTurning ? cornerPointDistance : normalPointDistance;

        // Check if we need to add a new point
        Vector3 newPos = GetPointBehindPlayer();
        if (Vector3.Distance(lastPosition, newPos) > distanceThreshold || isTurning)
        {
            // Add new point
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
            lastPosition = newPos;

            // Add simple box collider for this segment
            if (lineRenderer.positionCount >= 2)
            {
                Vector3 prevPos = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
                AddColliderBetweenPoints(prevPos, newPos);
            }

            // Update last direction
            lastDirection = currentDirection;
        }
    }

    Vector3 GetPointBehindPlayer()
    {
        // Calculate position behind the player based on player's rotation
        float angle = playerTransform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        // Place point behind the player
        return playerTransform.position - new Vector3(direction.x, direction.y, 0) * spawnDistance;
    }

    void AddColliderBetweenPoints(Vector3 start, Vector3 end)
    {
        // Create a new GameObject with a box collider
        GameObject collider = new GameObject("TrailCollider");
        collider.transform.parent = transform;
        collider.tag = "Trail";

        // Add box collider
        BoxCollider2D box = collider.AddComponent<BoxCollider2D>();
        box.isTrigger = true;

        // Position and rotate
        Vector3 midPoint = (start + end) / 2;
        collider.transform.position = midPoint;

        // Calculate direction and rotation
        Vector3 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        collider.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Set size
        float length = Vector3.Distance(start, end);
        box.size = new Vector2(length, lineWidth);
    }
}



