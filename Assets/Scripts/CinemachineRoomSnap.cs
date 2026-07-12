using UnityEngine;
using Unity.Cinemachine; 

public class CinemachineRoomSnap : MonoBehaviour
{
    private CinemachineCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
        
        // Force priority to 0 at the start of the game so no cameras are active yet
        vcam.Priority = 0; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player successfully entered: " + gameObject.name); // <--- DEBUG TEST
            vcam.Priority = 10;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left: " + gameObject.name); // <--- DEBUG TEST
            vcam.Priority = 0;
        }
    }
}