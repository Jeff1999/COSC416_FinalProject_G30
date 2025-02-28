using UnityEngine;

public class CrashAnimationController : MonoBehaviour
{
    public Sprite[] crashAnimationFrames; // Array to hold all explosion sprites
    public float animationFrameRate = 0.05f; // Time between frames
    public bool destroyAfterAnimation = true;

    // Add a public scale variable to control sprite size
    public float explosionScale = 2f; // Adjust this value to make the explosion larger or smaller

    private SpriteRenderer spriteRenderer;
    private float timer;
    private int currentFrame;
    private bool isAnimating = false;

    public void StartCrashAnimation(Vector3 crashPosition)
    {
        // Ensure we're not animating already
        if (isAnimating) return;

        // Create a new game object for the crash animation
        GameObject crashAnimationObject = new GameObject("CrashAnimation");
        crashAnimationObject.transform.position = crashPosition;

        // Add sprite renderer
        SpriteRenderer renderer = crashAnimationObject.AddComponent<SpriteRenderer>();
        renderer.sortingLayerName = "Effects";

        // Setup script
        CrashAnimationController animController = crashAnimationObject.AddComponent<CrashAnimationController>();

        // Directly copy the frames array
        animController.crashAnimationFrames = new Sprite[crashAnimationFrames.Length];
        for (int i = 0; i < crashAnimationFrames.Length; i++)
        {
            animController.crashAnimationFrames[i] = crashAnimationFrames[i];
        }

        // Set the initial sprite and mark as animating
        if (animController.crashAnimationFrames.Length > 0)
        {
            renderer.sprite = animController.crashAnimationFrames[0];

            // Set the scale of the explosion
            crashAnimationObject.transform.localScale = Vector3.one * animController.explosionScale;

            animController.isAnimating = true;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Effects";
        }

        // Do not set any sprite or start animation here
        spriteRenderer.sprite = null;
    }

    void Update()
    {
        if (!isAnimating || crashAnimationFrames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= animationFrameRate)
        {
            // Move to next frame
            currentFrame++;

            // Check if animation is complete
            if (currentFrame >= crashAnimationFrames.Length)
            {
                if (destroyAfterAnimation)
                {
                    Destroy(gameObject);
                }
                return;
            }

            // Update sprite
            spriteRenderer.sprite = crashAnimationFrames[currentFrame];

            // Reset timer
            timer = 0f;
        }
    }
}

