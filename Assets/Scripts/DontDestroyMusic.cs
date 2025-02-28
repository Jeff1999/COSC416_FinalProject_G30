using UnityEngine;

public class DontDestroyMusic : MonoBehaviour
{
    void Awake()
    {
        // Keep this object alive between scene loads
        DontDestroyOnLoad(gameObject);

        // Prevent duplicate music managers if scene reloads
        if (FindObjectsOfType<DontDestroyMusic>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}