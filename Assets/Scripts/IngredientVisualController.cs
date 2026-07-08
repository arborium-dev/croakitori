using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    // This is the magical STATIC list. Because it's static, there is only ONE 
    // copy of this list in the entire game, and all ingredients share it.
    public static List<Rigidbody2D> AllIngredientRigidbodies = new List<Rigidbody2D>();

    private Rigidbody2D _myRigidbody;

    void Awake()
    {
        // Get the rigidbody attached to this specific ingredient
        _myRigidbody = GetComponent<Rigidbody2D>();
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