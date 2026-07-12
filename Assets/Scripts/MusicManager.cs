// MusicManager.cs
// This manages music speeding up and slowing down in the cooking minigame
// Should not be used anywhere else

using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource music;
    public DoesEverything uiScript; // Reference to UI script

    [Header("Audio Settings")]
    public float normalSpeed = 1.0f;
    public float fastSpeed = 1.35f; // Adjust this to make it faster/slower
    public float timeThreshold = 10f; // When the music should speed up

    void Update()
    {
        // Make sure we have a reference to the UI script
        if (uiScript != null)
        {
            // If time is 10 seconds or less (but greater than 0)
            if (uiScript.CurrentTimeSeconds <= timeThreshold && uiScript.CurrentTimeSeconds > 0)
            {
                // Speed up the music
                music.pitch = fastSpeed;
                
                // alt version, speed up slowly
                // This calculates a percentage from 0 to 1 based on how close time is to 0
                // float timePercentage = uiScript.CurrentTimeSeconds / timeThreshold; 

                // music.pitch = Mathf.Lerp(fastSpeed, normalSpeed, timePercentage);
            }
            else if (uiScript.CurrentTimeSeconds > timeThreshold)
            {
                // Keep it at normal speed
                music.pitch = normalSpeed;
            }
            else if (uiScript.CurrentTimeSeconds <= 0)
            {
                // Optional: What happens when time runs out? 
                // music.Stop();
                music.pitch = normalSpeed; 
            }
        }
    }
}