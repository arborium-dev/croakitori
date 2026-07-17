using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TransitionToCookingIntro : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage != null)
        {
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime * 3;
                // Keep the color black, just change the alpha
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null; // Wait until next frame
            }
        }
        Debug.Log("Player entered CookingIntroTransition");
        // Load the CookingIntro scene
        SceneManager.LoadScene("CookingIntro");
    }
}
