// IngredientVisualController.cs
// This helps manage hitboxes and sprite sizes for spawning objects on the pan

using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    
    public static List<Rigidbody2D> AllIngredientRigidbodies = new List<Rigidbody2D>();

    private Rigidbody2D _myRigidbody;

    void Awake()
    {
        _myRigidbody = GetComponent<Rigidbody2D>();
    
        // Load random sprite from Resources folder
        Sprite[] ingredientSprites = Resources.LoadAll<Sprite>("Art/ingredients");
        if (ingredientSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, ingredientSprites.Length);
            Sprite randomSprite = ingredientSprites[randomIndex];
        
            // Assign sprite to SpriteRenderer
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = randomSprite;
        
            CircleCollider2D _collider = GetComponent<CircleCollider2D>();
        
            // Store your intended final world diameter (e.g., 1.0f if radius is 0.5f)
            float targetWorldDiameter = _collider.radius * 2f; 
        
            // Use the largest dimension of the sprite to prevent tall/wide sprites 
            // from sticking out of the circle
            float maxSpriteSize = Mathf.Max(randomSprite.bounds.size.x, randomSprite.bounds.size.y);
        
            // Scale the GameObject
            float scale = targetWorldDiameter / maxSpriteSize;
            transform.localScale = new Vector3(scale, scale, 1f);
        
            // FIX: Update the collider's local radius to match the sprite's local size.
            // Because the GameObject is being scaled, the final world size of the 
            // collider will perfectly equal your targetWorldDiameter!
            _collider.radius = maxSpriteSize / 2f;
        }
    }

    void OnEnable()
    {
        // When this object is summoned/turned on, add its Rigidbody to the list
        if (_myRigidbody != null)
        {
            AllIngredientRigidbodies.Add(_myRigidbody);
        }
    }

    void OnDisable()
    {
        // If this ingredient is destroyed or turned off, remove it from the list
        if (_myRigidbody != null)
        {
            AllIngredientRigidbodies.Remove(_myRigidbody);
        }
    }

    // --- NEW CODE ---
    // This runs automatically the moment this object leaves a "Trigger" area
    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object we just stopped touching has the tag "Pan"
        if (other.CompareTag("Pan"))
        {
            Debug.Log("Ingredient left the pan! Deleting...");
            
            // Delete this ingredient! 
            // (When this runs, OnDisable() will automatically remove it from the list)
            Destroy(gameObject);
        }
    }
}