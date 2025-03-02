using UnityEngine;

public class DontDestroyMusic : MonoBehaviour
{
    private static DontDestroyMusic instance;

    void Awake()
    {
        // If an instance already exists and it's not this, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, make this the instance and persist
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
