using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VideoPlayer))]
public class VideoToSceneTransition : MonoBehaviour
{
    public string sceneToLoad;

    private VideoPlayer videoPlayer;
    private InputAction skipCutscene;
    private bool hasTransitioned = false;

    [SerializeField] private InputActionAsset inputActions;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoEnd;

        skipCutscene = inputActions != null ? inputActions.FindAction("Skip", false) : null;
}

    void OnEnable()
    {
        skipCutscene?.Enable();
    }

    void OnDisable()
    {
        skipCutscene?.Disable();
    }

    void Update()
    {
        if (hasTransitioned)
            return;

        if (skipCutscene != null && skipCutscene.WasPressedThisFrame())
        {
            OnVideoEnd(videoPlayer);
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (hasTransitioned)
            return;

        hasTransitioned = true;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (SceneManager.GetActiveScene().name == "Ending")
            {
                PlayerPrefs.SetInt("GameComplete", 1);
                PlayerPrefs.Save();
            }

            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("VideoToSceneTransition OnVideoEnd() called without sceneToLoad");
            hasTransitioned = false;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}