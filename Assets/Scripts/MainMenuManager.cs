// MainMenuManager.cs
// Manages the main menu
// TODO: add video support to this script, as thats what the background for the main menu is (maybe this can be done in Unity itself?)

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI readingSpeedText;

    private bool gameComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // gameComplete = PlayerPrefs.GetInt("GameComplete", 0) == 1;
        // if (gameComplete == true)
        // {
        //     videoPlayer.clip = Resources.Load<VideoClip>("Cutscenes/croakitori main menu alt");
        // }    
        // Only set a default if the key doesn't exist
        // if (!PlayerPrefs.HasKey("ReadingSpeed"))
        //     PlayerPrefs.SetInt("ReadingSpeed", 1);

        // UpdateReadingSpeedText();
        // force reading speed to be slow
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void QuitGame()
    {
        if (gameComplete)
        {
            PlayerPrefs.SetInt("GameComplete", 0);
        }
        Application.Quit();
    }

    // public void ChangeReadingSpeed()
    // {
    //     int current = PlayerPrefs.GetInt("ReadingSpeed", 1);
    //     int next = (current == 1) ? 0 : 1;
    //     PlayerPrefs.SetInt("ReadingSpeed", next);
    //     PlayerPrefs.Save();
    //
    //     UpdateReadingSpeedText();
    //     Debug.Log($"Reading Speed: {next}");
    // }
    //
    // private void UpdateReadingSpeedText()
    // {
    //     if (readingSpeedText == null) return;
    //     bool isFast = PlayerPrefs.GetInt("ReadingSpeed", 0) == 1;
    //     readingSpeedText.text = isFast ? "Reading Speed: Fast" : "Reading Speed: Slow";
    // }

    public void SkipToCooking()
    {
        SceneManager.LoadScene("CookingIntro");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("OpeningScene");
        gameComplete  = false;
    }
    
    
}