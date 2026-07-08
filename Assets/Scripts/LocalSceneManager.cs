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
