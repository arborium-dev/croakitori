// UNUSED
using UnityEngine;
using UnityEngine.InputSystem;

public class TheBigDrag : MonoBehaviour
{
    private Vector3 theBigOffset;
    private Camera mainCamera;
    private bool isDragging = false;

    private void Start()
    {
        mainCamera = Camera.main;
        Debug.Log($"{gameObject.name} initialized TheBigDrag script.");
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Left mouse button pressed.");
            Vector3 mousePos = GetMouseWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
                
                // Check if the raycast hit the collider attached to this GameObject
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log($"Started dragging {gameObject.name}");
                    isDragging = true;
                    theBigOffset = transform.position - mousePos;
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing.");
            }
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            if (isDragging)
            {
                Debug.Log($"Stopped dragging {gameObject.name}");
            }
            isDragging = false;
        }

        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + theBigOffset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, -mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}