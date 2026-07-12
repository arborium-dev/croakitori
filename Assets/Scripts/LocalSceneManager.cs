// LocalSceneManager.cs
// ONLY TO BE USED IN THE COOKING SCENE, this script passes of the score to the ending and switches the scene to the ending
// should probably rename this file tbh, considering it litterally only has one purpose

using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalSceneManager : MonoBehaviour
{
    public static LocalSceneManager Instance { get; private set; }

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
        TotalScore = totalScore;
        Debug.Log($"SceneManager received final total score: {TotalScore}");
        // switch scene to Ending and give it TotalScore
        SceneManager.LoadScene("Ending");
    }
}
