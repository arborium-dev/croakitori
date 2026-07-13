// PlayerPlatforming.cs
// This script controls how the player physics and collisions function

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlatforming : MonoBehaviour
{
    private const float WallNormalThreshold = 0.1f;
    private const float FloorNormalThreshold = 0.1f;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 8f;

    [Tooltip("How fast the player reaches top speed from a standstill")]
    [SerializeField]
    private float acceleration = 80f;

    [Tooltip("How fast the player stops when there is no input")]
    [SerializeField]
    private float deceleration = 60f;

    [Tooltip("How fast the player changes direction when moving opposite to input")]
    [SerializeField]
    private float turnSpeed = 120f;

    [SerializeField]
    private float jumpSpeed = 12f;

    [SerializeField]
    private float gravity = 45f;

    [SerializeField]
    private float maxFallSpeed = 22f;

    [Header("Jump Forgiveness")]
    [SerializeField]
    private float coyoteTime = 0.1f; 

    [SerializeField]
    private float jumpBufferTime = 0.1f;
    
    [Tooltip("Multiplier applied to upward velocity when the jump button is released early. Lower = shorter hops.")]
    [SerializeField, Range(0f, 1f)]
    private float jumpCutMultiplier = 0.5f;

    [Header("Collision")]
    [SerializeField]
    private LayerMask groundLayer;
    
    [SerializeField]
    private float skinWidth = 0.02f;

    [SerializeField]
    private float groundProbeDistance = 0.08f;

    [Header("Input")]
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private InputActionAsset inputActions;

    [SerializeField]
    private string moveActionName = "Move";

    [SerializeField]
    private string jumpActionName = "Jump";


    private InputAction _moveAction;
    private InputAction _jumpAction;
   

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private ContactFilter2D _collisionFilter;
    private readonly RaycastHit2D[] _castHits = new RaycastHit2D[8];

    private float _moveInput;
    private bool _isJumpHeld;
  
    private Vector2 _velocity;
    
    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _ownsEnabledActions;
    private bool _isJumping;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        _collisionFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = groundLayer,
            useTriggers = false
        };

        ResolveActions();
    }

    private void OnEnable()
    {
        ResolveActions();

        // PlayerInput manages its own action enable state.
        if (playerInput == null && _moveAction != null && _jumpAction != null)
        {
            _moveAction.Enable();
            _jumpAction.Enable();
           
            _ownsEnabledActions = true;
        }
    }

    private void OnDisable()
    {
        if (_ownsEnabledActions)
        {
            _moveAction.Disable();
            _jumpAction.Disable();
            
            _ownsEnabledActions = false;
        }
    }

    private void Update()
    {
        _moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>().x : 0f;
        
        // Track whether the jump button is currently being held down
        _isJumpHeld = _jumpAction != null && _jumpAction.IsPressed();
        
        if (_jumpAction != null && _jumpAction.WasPressedThisFrame())
        {
            _jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            _jumpBufferTimer -= Time.deltaTime;
        }

        _coyoteTimer -= Time.deltaTime;
    }
    
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if (IsGrounded())
        {
            _coyoteTimer = coyoteTime;
            if (_velocity.y < 0f)
            {
                _velocity.y = 0f;
                _isJumping = false; // reset jumping state when grounded
            }
        }

        IsTouchingHazard();

        // initializing the jump
        if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
        {
            _velocity.y = jumpSpeed;
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            _isJumping = true; // we are now actively in a jump arc
        }

        // --- VARIABLE JUMP HEIGHT ---
        // If we are jumping up, but the player let go of the jump button early...
        if (_isJumping && !_isJumpHeld && _velocity.y > 0f)
        {
            // Cut the upward velocity 
            _velocity.y *= jumpCutMultiplier;
            
            // Set to false so we only cut the velocity once per jump
            _isJumping = false; 
        }

        // If the player begins falling naturally, they are no longer in an upward jump arc
        if (_velocity.y <= 0f)
        {
            _isJumping = false;
        }

        // --- HORIZONTAL MOVEMENT ---
        float targetVelocityX = _moveInput * moveSpeed;
        float accelRate;

        if (Mathf.Abs(_moveInput) < 0.01f)
        {
            accelRate = deceleration;
        } 
        else if (Mathf.Abs(_velocity.x) > 0.01f && Mathf.Sign(_moveInput) != Mathf.Sign(_velocity.x))
        {
            accelRate = turnSpeed;
        }
        else
        {
            accelRate = acceleration;
        }

        _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocityX, accelRate * dt);
        
        // --- GRAVITY ---
        _velocity.y = Mathf.Max(_velocity.y - (gravity * dt), -maxFallSpeed);
        
        Vector2 delta = _velocity * dt;
        MoveHorizontal(ref delta.x);
        MoveVertical(ref delta.y);

        _rb.MovePosition(_rb.position + delta);
    }

    private void MoveHorizontal(ref float moveX)
    {
        if (Mathf.Abs(moveX) < 0.0001f)
        {
            return;
        }

        float direction = Mathf.Sign(moveX);
        float distance = Mathf.Abs(moveX) + skinWidth;
        int hitCount = _rb.Cast(new Vector2(direction, 0f), _collisionFilter, _castHits, distance);

        for (int i = 0; i < hitCount; i++)
        {
            // Ignore floor/ceiling contacts when resolving horizontal movement.
            if (Mathf.Abs(_castHits[i].normal.x) < WallNormalThreshold)
            {
                continue;
            }

            float allowed = Mathf.Max(0f, _castHits[i].distance - skinWidth);
            if (allowed < Mathf.Abs(moveX))
            {
                moveX = allowed * direction;
                _velocity.x = 0f;
            }
        }
    }

    private void MoveVertical(ref float moveY)
    {
        if (Mathf.Abs(moveY) < 0.0001f)
        {
            return;
        }

        float direction = Mathf.Sign(moveY);
        float distance = Mathf.Abs(moveY) + skinWidth;
        int hitCount = _rb.Cast(new Vector2(0f, direction), _collisionFilter, _castHits, distance);

        for (int i = 0; i < hitCount; i++)
        {
            // Ignore near-vertical-wall contacts when resolving vertical movement.
            if (Mathf.Abs(_castHits[i].normal.y) < FloorNormalThreshold)
            {
                continue;
            }

            float allowed = Mathf.Max(0f, _castHits[i].distance - skinWidth);
            if (allowed < Mathf.Abs(moveY))
            {
                moveY = allowed * direction;
                _velocity.y = 0f;
            }
        }
    }

    private bool IsGrounded()
    {
        int hitCount = _rb.Cast(Vector2.down, _collisionFilter, _castHits, groundProbeDistance);
        for (int i = 0; i < hitCount; i++)
        {
            if (_castHits[i].normal.y > FloorNormalThreshold)
            {
                return true;
            }
        }

        return false;
    }
    

    private void ResolveActions()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        if (playerInput != null && playerInput.actions != null)
        {
            _moveAction = playerInput.actions.FindAction(moveActionName);
            _jumpAction = playerInput.actions.FindAction(jumpActionName);
            
        }

        if ((_moveAction == null || _jumpAction == null) && inputActions != null)
        {
            _moveAction ??= inputActions.FindAction(moveActionName);
            _jumpAction ??= inputActions.FindAction(jumpActionName);
            
        }

        if (_moveAction == null || _jumpAction == null)
        {
            _moveAction ??= InputSystem.actions?.FindAction(moveActionName);
            _jumpAction ??= InputSystem.actions?.FindAction(jumpActionName);
            
        }

        if (_moveAction == null || _jumpAction == null)
        {
            Debug.LogWarning("PlayerPlatforming: Could not find Move/Jump actions. Assign PlayerInput or InputActionAsset in the inspector.", this);
        }
    }

    private bool IsTouchingHazard()
    {
        // check every direction
        Vector2[] directions = { Vector2.down, Vector2.up, Vector2.left, Vector2.right };

        foreach (Vector2 direction in directions)
        {
            // raycasting logic
            int hitCount = _rb.Cast(direction, _collisionFilter, _castHits, groundProbeDistance);

            for (int i = 0; i < hitCount; i++)
            {
                // does something have the hazard tag?
                if (_castHits[i].collider.CompareTag("Hazard"))
                {
                    Debug.Log("IsTouchingHazard: true");
                    return true;
                }
            }
        }
        return false;
    }
}