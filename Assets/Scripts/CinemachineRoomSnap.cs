using UnityEngine;
using Unity.Cinemachine; // The namespace changed in the new version

public class CinemachineRoomSnap : MonoBehaviour
{
    // The component is now called CinemachineCamera instead of CinemachineVirtualCamera
    private CinemachineCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Boost this camera's priority so Cinemachine switches to it
            vcam.Priority = 10;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Drop priority when the player leaves so the next room can take over
            vcam.Priority = 0;
        }
    }
}