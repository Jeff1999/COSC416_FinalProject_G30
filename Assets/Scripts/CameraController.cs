using UnityEngine;

public class CameraFitWithEqualBorders : MonoBehaviour
{
    [SerializeField] private float arenaWidth = 40f;
    [SerializeField] private float arenaHeight = 30f;
    [SerializeField] private float padding = 2f; // Optional padding around the arena
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        AdjustCamera();
    }

    void AdjustCamera()
    {
        // Add padding to dimensions
        float targetWidth = arenaWidth + (padding * 2);
        float targetHeight = arenaHeight + (padding * 2);

        // Get the aspect ratio
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = targetWidth / targetHeight;

        if (screenRatio >= targetRatio)
        {
            // Screen is wider than target - fit height and center horizontally
            cam.orthographicSize = targetHeight * 0.5f;
        }
        else
        {
            // Screen is taller than target - fit width and center vertically
            cam.orthographicSize = targetWidth * 0.5f / screenRatio;
        }

        Debug.Log("Camera size set to: " + cam.orthographicSize);
    }

    void Update()
    {
        // Check if aspect ratio changes
        float currentAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(currentAspect - cam.aspect) > 0.01f)
        {
            AdjustCamera();
        }
    }
}
