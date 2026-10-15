using UnityEngine;
using UnityEngine.UI;

public class IfMambo : MonoBehaviour
{
    public AudioSource _mainMusic;
    public AudioSource _mamboMusic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            startMusic();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            endMusic();
        }
    }

    private void startMusic()
    {
        _mainMusic.Stop();
        _mamboMusic.Play();
    }

    private void endMusic()
    {
        _mamboMusic.Stop();
        _mainMusic.Play();
    }
}