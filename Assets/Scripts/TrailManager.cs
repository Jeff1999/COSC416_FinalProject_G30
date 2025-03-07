using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailManager : MonoBehaviour
{
    // Reference to your WorkingTrailGenerator script
    private MonoBehaviour trailGenerator; // Changed to MonoBehaviour type

    // Reference to the GameObject with the trail generator
    private GameObject trailObject;

    // Keep track of all created trail segments
    private List<GameObject> trailSegments = new List<GameObject>();

    // Debug visualization
    public bool showDebugInfo = true;
    public Color jumpStartColor = Color.red;
    public Color jumpEndColor = Color.green;

    void Awake()
    {
        // Find the trail generator by name rather than by type
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            // Find the child with the trail generator
            Component[] components = child.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component.GetType().Name.Contains("TrailGenerator"))
                {
                    Debug.Log("Found TrailGenerator on: " + child.name);
                    trailGenerator = component;
                    trailObject = child.gameObject;
                    break;
                }
            }

            if (trailGenerator != null) break;
        }

        if (trailGenerator == null)
        {
            Debug.LogError("TrailManager: No trail generator script found on any child objects! Make sure your script name contains 'TrailGenerator'");
        }
        else
        {
            Debug.Log("TrailManager: Successfully found trail generator: " + trailGenerator.GetType().Name);
        }
    }

    public void ResetTrail()
    {
        if (trailGenerator == null || trailObject == null)
        {
            Debug.LogError("TrailManager: Cannot reset trail - trail generator not found!");
            return;
        }

        Vector3 currentPos = transform.position;
        Debug.Log("ResetTrail called at position: " + currentPos);

        // Debug visualization at jump points
        if (showDebugInfo)
        {
            // Mark jump start point
            GameObject startMarker = CreateDebugMarker(currentPos, jumpStartColor, "JumpStart");
            GameObject.Destroy(startMarker, 3f); // Remove after 3 seconds
        }

        // Get the current LineRenderer
        LineRenderer currentLine = trailObject.GetComponent<LineRenderer>();

        if (currentLine != null && currentLine.positionCount > 0)
        {
            Debug.Log("Current line has " + currentLine.positionCount + " points");

            // Create a permanent copy of the current trail
            GameObject permanentTrail = new GameObject("PermanentTrail_" + trailSegments.Count);
            permanentTrail.transform.position = Vector3.zero;
            permanentTrail.tag = "Trail";
            trailSegments.Add(permanentTrail);

            // Add line renderer and copy current trail
            LineRenderer permLine = permanentTrail.AddComponent<LineRenderer>();
            CopyLineRendererProperties(currentLine, permLine);

            // Copy all points
            Vector3[] positions = new Vector3[currentLine.positionCount];
            currentLine.GetPositions(positions);
            permLine.positionCount = positions.Length;
            permLine.SetPositions(positions);

            Debug.Log("Created permanent trail with " + permLine.positionCount + " points");

            // Add colliders
            AddCollidersToTrail(permanentTrail, positions, currentLine.startWidth);
        }

        // Disable the current trail object completely
        trailObject.SetActive(false);

        // Create a brand new trail generator after the jump
        StartCoroutine(CreateNewTrailAfterJump());
    }

    private IEnumerator CreateNewTrailAfterJump()
    {
        // Wait for a frame to make sure the player has moved
        yield return null;

        Vector3 newPos = transform.position;

        // Debug for new position
        if (showDebugInfo)
        {
            // Mark jump end point
            GameObject endMarker = CreateDebugMarker(newPos, jumpEndColor, "JumpEnd");
            GameObject.Destroy(endMarker, 3f); // Remove after 3 seconds
            Debug.Log("Jump ending at position: " + newPos);
        }

        // Create a completely new GameObject for the new trail
        GameObject newTrailObj = new GameObject("NewTrail_" + trailSegments.Count);
        newTrailObj.transform.parent = transform; // Make it a child of the player
        newTrailObj.transform.position = Vector3.zero;
        newTrailObj.tag = "Trail";

        // Add a line renderer
        LineRenderer newLine = newTrailObj.AddComponent<LineRenderer>();

        // Get original properties to copy
        LineRenderer originalLine = trailObject.GetComponent<LineRenderer>();
        if (originalLine != null)
        {
            CopyLineRendererProperties(originalLine, newLine);
        }
        else
        {
            // Default setup if original not found
            newLine.startWidth = 0.2f;
            newLine.endWidth = 0.2f;
            newLine.material = new Material(Shader.Find("Sprites/Default"));
            newLine.startColor = Color.cyan;
            newLine.endColor = Color.cyan;
        }

        // Set up the new LineRenderer with just the starting point
        newLine.positionCount = 1;
        newLine.SetPosition(0, GetPointBehindPlayer());

        Debug.Log("Created new trail at position: " + newPos);

        // Add a WorkingTrailGenerator component by copying it
        // We'll copy the component manually using reflection
        System.Type trailGenType = trailGenerator.GetType();
        MonoBehaviour newTrailGen = (MonoBehaviour)newTrailObj.AddComponent(trailGenType);

        // Set basic properties (these property names should match your WorkingTrailGenerator)
        SetPropertyValue(newTrailGen, "playerTransform", transform);
        SetPropertyValue(newTrailGen, "lineWidth", GetPropertyValue(trailGenerator, "lineWidth"));
        SetPropertyValue(newTrailGen, "trailColor", GetPropertyValue(trailGenerator, "trailColor"));
        SetPropertyValue(newTrailGen, "spawnDistance", GetPropertyValue(trailGenerator, "spawnDistance"));

        try
        {
            // Try to set these properties if they exist
            SetPropertyValue(newTrailGen, "normalPointDistance", GetPropertyValue(trailGenerator, "normalPointDistance"));
            SetPropertyValue(newTrailGen, "cornerPointDistance", GetPropertyValue(trailGenerator, "cornerPointDistance"));
        }
        catch (System.Exception)
        {
            // Ignore if these properties don't exist
        }

        // Replace references
        trailGenerator = newTrailGen;
        trailObject = newTrailObj;

        Debug.Log("New trail generator created and initialized");
    }

    // Helper to get a point behind the player (mimics your WorkingTrailGenerator logic)
    private Vector3 GetPointBehindPlayer()
    {
        float spawnDistance = 0.5f;
        try
        {
            // Try to get the actual spawn distance from the trail generator
            spawnDistance = (float)GetPropertyValue(trailGenerator, "spawnDistance");
        }
        catch
        {
            // Use the default if we can't get it
        }

        // Calculate position behind the player based on player's rotation
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        // Place point behind the player
        return transform.position - new Vector3(direction.x, direction.y, 0) * spawnDistance;
    }

    private void CopyLineRendererProperties(LineRenderer source, LineRenderer target)
    {
        if (source == null || target == null) return;

        target.startWidth = source.startWidth;
        target.endWidth = source.endWidth;
        target.material = new Material(source.material);
        target.startColor = source.startColor;
        target.endColor = source.endColor;
    }

    private void AddCollidersToTrail(GameObject parent, Vector3[] points, float width)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            GameObject collider = new GameObject("TrailCollider_" + i);
            collider.transform.parent = parent.transform;
            collider.tag = "Trail";

            BoxCollider2D box = collider.AddComponent<BoxCollider2D>();
            box.isTrigger = true;

            Vector3 midPoint = (points[i] + points[i + 1]) / 2;
            collider.transform.position = midPoint;

            Vector3 direction = points[i + 1] - points[i];
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            collider.transform.rotation = Quaternion.Euler(0, 0, angle);

            float length = Vector3.Distance(points[i], points[i + 1]);
            box.size = new Vector2(length, width);
        }
    }

    private GameObject CreateDebugMarker(Vector3 position, Color color, string name)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = name;
        marker.transform.position = position;
        marker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Set color
        Renderer renderer = marker.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }

        return marker;
    }

    // Helper method to get property value using reflection
    private object GetPropertyValue(object obj, string propertyName)
    {
        System.Type type = obj.GetType();
        System.Reflection.PropertyInfo prop = type.GetProperty(propertyName);
        if (prop != null)
        {
            return prop.GetValue(obj);
        }

        // Try to get field if property doesn't exist
        System.Reflection.FieldInfo field = type.GetField(propertyName);
        if (field != null)
        {
            return field.GetValue(obj);
        }

        throw new System.Exception("Property or field not found: " + propertyName);
    }

    // Helper method to set property value using reflection
    private void SetPropertyValue(object obj, string propertyName, object value)
    {
        System.Type type = obj.GetType();
        System.Reflection.PropertyInfo prop = type.GetProperty(propertyName);
        if (prop != null)
        {
            prop.SetValue(obj, value);
            return;
        }

        // Try to set field if property doesn't exist
        System.Reflection.FieldInfo field = type.GetField(propertyName);
        if (field != null)
        {
            field.SetValue(obj, value);
            return;
        }

        Debug.LogWarning("Property or field not found when setting: " + propertyName);
    }
}

