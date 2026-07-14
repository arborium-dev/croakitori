// ParallaxBackground.cd
// not used i think?
// part of ParallaxGenerator.cs, and discontinued for the same reason
// not deleting it because I think it might be important
// todo: delete

using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [HideInInspector] public float length; // Now set automatically by the Generator
    private float startpos;
    
    [Header("Assign the MAIN CAMERA here")]
    public GameObject cam;
    
    [Header("Parallax Settings")]
    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position.x;
    }

    void LateUpdate()
    {
        // Calculate movement relative to the camera
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        // Move the layer
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // --- INFINITE LOOPING LOGIC ---
        if (temp > startpos + length)
        {
            startpos += length;
        }
        else if (temp < startpos - length)
        {
            startpos -= length;
        }
    }
}