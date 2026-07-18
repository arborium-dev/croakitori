using UnityEngine;
using UnityEngine.Video;

public class OpeningSceneChanger : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [SerializeField] private VideoClip slowVideo;
    [SerializeField] private VideoClip fastVideo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int readingSpeed = PlayerPrefs.GetInt("ReadingSpeed", 1); // choose the defaulthere
        Debug.Log($"OpeningSceneChanger: ReadingSpeed = {readingSpeed}");
        bool isFast = readingSpeed == 1;
        videoPlayer.clip = isFast ? fastVideo : slowVideo;
        videoPlayer.Play();
    }

}
