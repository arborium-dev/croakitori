using UnityEngine;

public class EndingManager : MonoBehaviour
{
    private int _totalScore;

    void Start()
    {
        if (LocalSceneManager.Instance != null)
        {
            _totalScore = LocalSceneManager.Instance.TotalScore;
            Debug.Log($"Final total score: {_totalScore}");
        }
        else
        {
            Debug.LogWarning("LocalSceneManager.Instance was null.");
        }
    }
}