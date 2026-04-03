using UnityEngine;
using UnityEngine.InputSystem;  // 1. The Input System "using" statement

public class Example : MonoBehaviour
{
    [SerializeField] 
    private float moveSpeed = 8f;

    [SerializeField] 
    private float jumpForce = 300f;
    
    
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private Rigidbody2D _rb;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // 3. Find the references to the "Move" and "Jump" actions
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        _rb.linearVelocity = new Vector2(moveValue.x * moveSpeed, _rb.linearVelocity.y);

        if (_jumpAction.WasPressedThisFrame())
        {
            _rb.AddForce(Vector2.up * jumpForce);
        }
    }
}