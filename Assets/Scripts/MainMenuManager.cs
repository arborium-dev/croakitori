// MainMenuManager.cs
// Manages the main menu
// TODO: add video support to this script, as thats what the background for the main menu is (maybe this can be done in Unity itself?)

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SkipToCooking()
    {
        SceneManager.LoadScene("CookingIntro");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("OpeningScene");
    }
}