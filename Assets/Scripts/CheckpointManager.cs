using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    
    // other scripts can access this
    public static CheckpointManager Instance;
    
    // stores value
    [HideInInspector] public Vector2 currentCheckpoint;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector2 newPosition)
    {
        currentCheckpoint = newPosition;
        // could as visual effect?
    }
}
