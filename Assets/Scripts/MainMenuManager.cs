// MainMenuManager.cs
// Manages the main menu
// TODO: add video support to this script, as thats what the background for the main menu is (maybe this can be done in Unity itself?)

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private bool gameComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameComplete = PlayerPrefs.GetInt("GameComplete", 0) == 1;
        if (gameComplete == true)
        {
            videoPlayer.clip = Resources.Load<VideoClip>("Cutscenes/croakitori main menu alt");
        }    
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