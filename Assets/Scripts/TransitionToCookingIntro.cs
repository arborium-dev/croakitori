using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToCookingIntro : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered CookingIntroTransition");
            // Load the CookingIntro scene
            SceneManager.LoadScene("CookingIntro");
        }
    }
}
