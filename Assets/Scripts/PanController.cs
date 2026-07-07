using UnityEngine;
using UnityEngine.InputSystem;

public class PanController : MonoBehaviour
{
    InputAction moveAction;
    Vector2 moveValue = Vector2.zero;

    [Header("Movement Settings")]
    [Tooltip("How strongly the ingredients are pulled to the center of the pan")]
    public float centerPullForce = 5f; 
    
    [Tooltip("How strongly the player's input pushes the ingredients around")]
    public float inputPushForce = 15f;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("move");
    }

    void Update()
    {
        // Read the input every frame
        if (moveAction != null)
        {
            moveValue = moveAction.ReadValue<Vector2>();
        }
    }

    // FixedUpdate runs in sync with the Physics Engine. 
    // ALWAYS put continuous AddForce calls in here!
    void FixedUpdate()
    {
        foreach (Rigidbody2D rb in Ingredient.allIngredientRigidbodies)
        {
            // Safety check in case an ingredient is in the middle of being destroyed
            if (rb == null) continue; 

            // 1. PULL TOWARDS CENTER OF THE PAN
            // Calculate the direction from the ingredient to the pan (transform.position)
            Vector2 directionToCenter = ((Vector2)transform.position - rb.position).normalized;
            
            // Apply a continuous force pulling them to the center
            // Notice we aren't using Impulse here, just a standard smooth force
            rb.AddForce(directionToCenter * centerPullForce);

            // 2. APPLY PLAYER INPUT (Sloshing around)
            // Apply the player's movement input as a force to slosh them around in the pan
            rb.AddForce(moveValue * inputPushForce);
        }
    }
}