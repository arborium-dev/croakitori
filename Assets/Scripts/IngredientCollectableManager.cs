// IngredientCollectableManager.cs
// this manages the collectable spices around the level (how they follow, and the text effect after)
// in the code they're called ingredients because im indecisive

using UnityEngine;
using TMPro;

public class IngredientCollectableManager : MonoBehaviour
{
    [Header("Idle Animation")] 
    [SerializeField]
    private float bobSpeed = 3f;

    [SerializeField] private float bobHeight = 0.25f;

    [Header("Following")] [SerializeField] private float followSpeed = 8f;
    [SerializeField] private Vector3 followOffset = new Vector3(-0.5f, 0.5f, 0f);

    [Header("Collection Popup")]
    [Tooltip("The TextMeshPro object that acts as the popup")]
    [SerializeField] private TextMeshPro popupText;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float fadeSpeed = 2f;
    
    private bool _isFollowing;
    private Vector3 _startPosition;

    private Transform _playerTransform;
    private bool _isCollected;
    private PlayerPlatforming _playerScript;

    private float _followTimer;
    
    private void Start()
    {
        // Remember where we started so we can bob up and down from this point
        _startPosition = transform.position;
        
        // Ensure the popup text is hidden when the game starts
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // --- COLLECTED (FLOAT & FADE) STATE ---
        if (_isCollected)
        {
            // 1. Float upwards
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            // 2. Fade out the text
            if (popupText != null)
            {
                Color c = popupText.color;
                c.a -= fadeSpeed * Time.deltaTime;
                popupText.color = c;

                // 3. Destroy once fully invisible
                if (c.a <= 0f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // Fallback in case you forgot to assign the Text in the inspector
                Destroy(gameObject);
            }
            
            return; // Skip the rest of the Update loop
        }

        // --- IDLE BOBBING ---
        if (!_isFollowing)
        {
            float newY = _startPosition.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
            transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
        }
        
        // --- FOLLOWING PLAYER ---
        else
        {
            _followTimer += Time.deltaTime;

            Vector3 targetPosition = _playerTransform.position + followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
            // LERP? MORE LIKE LARP LIKE IM LARPING THIS CODE BECAUSEI DONT KNOW HOW IT WORKS
            // im sorry that was really bad
            // ill get back to work

            if (_followTimer > 0.15f && _playerScript != null && _playerScript.IsGrounded())
            {
                Collect();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!_isFollowing && col.CompareTag("Player"))
        {
            _isFollowing = true;
            _playerTransform = col.transform;
            _playerScript = col.GetComponent<PlayerPlatforming>();
            
            // Subscribe to player death event
            if (_playerScript != null)
            {
                _playerScript.OnPlayerDeath += ResetToStart;
            }
            
            // todo: put some kind of grapped sfx here
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the death event to avoid memory leaks
        if (_playerScript != null)
        {
            _playerScript.OnPlayerDeath -= ResetToStart;
        }
    }

    private void ResetToStart()
    {
        // Stop following and return to idle state
        _isFollowing = false;
        _followTimer = 0f;
        transform.position = _startPosition;
        
        // Re-enable sprite and collider if they were disabled
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        
        // Hide the popup text
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }

    private void Collect()
    {
        Debug.Log("Ingredient collected and banked");
        // 1. Swap states
        _isCollected = true;
        _isFollowing = false;

        // 2. Disable the Sprite Renderer so the jar vanishes
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // 3. Disable the Collider so it doesn't accidentally trigger anything else
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 4. Turn on the popup text and ensure its alpha is fully opaque
        if (popupText != null)
        {
            popupText.gameObject.SetActive(true);
            
            Color c = popupText.color;
            c.a = 1f;
            popupText.color = c;
        }
    }

}