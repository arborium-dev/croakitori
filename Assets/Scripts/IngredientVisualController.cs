using System.Collections.Generic; // We need this line to use Lists!
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    // This is the magical STATIC list. Because it's static, there is only ONE 
    // copy of this list in the entire game, and all ingredients share it.
    public static List<Rigidbody2D> allIngredientRigidbodies = new List<Rigidbody2D>();

    private Rigidbody2D myRigidbody;

    void Awake()
    {
        // Get the rigidbody attached to this specific ingredient
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // When this object is summoned/turned on, add its Rigidbody to the list
        if (myRigidbody != null)
        {
            allIngredientRigidbodies.Add(myRigidbody);
        }
    }

    void OnDisable()
    {
        // If this ingredient is destroyed or turned off, remove it from the list
        
        if (myRigidbody != null)
        {
            allIngredientRigidbodies.Remove(myRigidbody);
        }
    }
}