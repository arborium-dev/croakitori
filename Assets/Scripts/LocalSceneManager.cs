// LocalSceneManager.cs
// ONLY TO BE USED IN THE COOKING SCENE, this script passes of the score to the ending and switches the scene to the ending
// should probably rename this file tbh, considering it litterally only has one purpose

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LocalSceneManager : MonoBehaviour
{
    public static LocalSceneManager Instance { get; private set; }
    [SerializeField] private Image fadeImage;

    public int TotalScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ReceiveTotalScore(int totalScore)
    {
        StartCoroutine(FadeOut(totalScore));
    }

    private IEnumerator FadeOut(int totalScore)
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
        
        TotalScore = totalScore;
        Debug.Log($"SceneManager received final total score: {TotalScore}");

        if (TotalScore == -1)
        {
            SceneManager.LoadScene("CookingIntro");
            yield break;
        }
        // switch scene to Ending and give it TotalScore
        if (TotalScore != -1)
        {
            SceneManager.LoadScene("Ending");            
        }

    }
}
