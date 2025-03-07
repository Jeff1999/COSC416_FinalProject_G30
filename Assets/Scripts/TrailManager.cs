using UnityEngine;
// Add this new script to your project
public class TrailManager : MonoBehaviour
{
    // Reference to your actual TrailGenerator script
    private MonoBehaviour trailGenerator;
    // Reference to the line renderer
    private LineRenderer lineRenderer;
    void Awake()
    {
        // Find the trail generator and line renderer components
        Component[] components = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component.GetType().Name == "TrailGenerator")
            {
                trailGenerator = component;
                break;
            }
        }
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Call this to completely reset the trail (creates a new trail segment)
    public void ResetTrail()
    {
        // 1. Disable the trail generator
        if (trailGenerator != null)
        {
            trailGenerator.enabled = false;
        }
        // 2. Create a new GameObject for the old trail
        if (lineRenderer != null)
        {
            // Get current positions from line renderer
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            if (positions.Length > 0)
            {
                // Create a new GameObject to hold the old trail
                GameObject oldTrail = new GameObject("OldTrail");
                oldTrail.transform.position = Vector3.zero;
                oldTrail.tag = "Trail"; // Set the same tag as your normal trail
                // Add a line renderer to the new GameObject
                LineRenderer oldLineRenderer = oldTrail.AddComponent<LineRenderer>();
                // Copy properties from the current line renderer
                oldLineRenderer.startWidth = lineRenderer.startWidth;
                oldLineRenderer.endWidth = lineRenderer.endWidth;
                oldLineRenderer.material = lineRenderer.material;
                oldLineRenderer.startColor = lineRenderer.startColor;
                oldLineRenderer.endColor = lineRenderer.endColor;
                // Set the positions
                oldLineRenderer.positionCount = positions.Length;
                oldLineRenderer.SetPositions(positions);
                // Add a collider if needed
                if (positions.Length >= 2)
                {
                    EdgeCollider2D collider = oldTrail.AddComponent<EdgeCollider2D>();
                    Vector2[] edgePoints = new Vector2[positions.Length];
                    for (int i = 0; i < positions.Length; i++)
                    {
                        edgePoints[i] = new Vector2(positions[i].x, positions[i].y);
                    }
                    collider.points = edgePoints;
                    collider.isTrigger = true;
                }
            }
            // Clear the current line renderer
            lineRenderer.positionCount = 0;
        }
        // 3. Re-enable the trail generator
        if (trailGenerator != null)
        {
            trailGenerator.enabled = true;
        }
    }
}


