using UnityEngine;

public class TeleportEasterEgg : MonoBehaviour
{

    public Transform teleportLocation;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.transform.position = teleportLocation.position;
        }
    }
}
