// THIS CODE IS ALL UNTESTED

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    private int _totalScore;
    public VideoPlayer  finalCutscene;

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
        
        PlayEndingCutscene(_totalScore);
    }

    private void PlayEndingCutscene(int score)
    {
        // decide on cutscene based on score
        string cutsceneFileName = GetCutsceneName(score);
        
        // load from resource folder
        VideoClip clip = Resources.Load<VideoClip>("Cutscenes/" + cutsceneFileName);

        if (clip != null)
        {
            // assign video
            finalCutscene.clip = clip;
            
            // make sure audio plays

            finalCutscene.audioOutputMode = VideoAudioOutputMode.Direct;
            finalCutscene.EnableAudioTrack(0, true);
            
            // when does the cutscene even end?
            
            finalCutscene.isLooping = false;
            finalCutscene.loopPointReached += OnCutsceneEnded;
            
            // let the show begin!
            finalCutscene.Play();
        }
        else
        {
            Debug.LogError($"YO CHAT, cutscene not found at Resources/Cutscenes/{cutsceneFileName}.mp4");
            //optional failsafe:
            //ResetGame();
        }
    }

    private string GetCutsceneName(int score)
    {
        if (score == -1) return "timerEnding";
        if (score == 0) return "badEnding";
        if (0 < score && score != 15) return "normalEnding";
        if (score >= 15) return "goodEnding";
        
        return "errorEnding"; // this is to catch any score values that dont trigger any of the above endings, although that should be impossible
    }

    private void OnCutsceneEnded(VideoPlayer vp)
    {
        // memory leak mmmm
        vp.loopPointReached -= OnCutsceneEnded;

        Debug.Log("Cutscene finished!");
        ResetGame();
    }

    private void ResetGame()
    {
        Debug.Log("resetting game");
        
        // THERE SHOULD PROBABLY BE MORE HERE TO RESET GAME, OR TO HAVE A POST GAME SECRET...
        
        SceneManager.LoadScene(0);
    }
    
}
