using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlatforming : MonoBehaviour
{
    private const float WallNormalThreshold = 0.1f;
    private const float FloorNormalThreshold = 0.1f;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 8f;

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

    [Header("Collision")]
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float skinWidth = 0.02f;

    [SerializeField]
    private float groundProbeDistance = 0.08f;

    [SerializeField]
    private float wallProbeDistance = 0.08f;

    [Header("Climb")]
    [SerializeField]
    private float climbSpeed = 6f;

    [Header("Input")]
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private InputActionAsset inputActions;

    [SerializeField]
    private string moveActionName = "Move";

    [SerializeField]
    private string jumpActionName = "Jump";

    [SerializeField]
    private string climbActionName = "Climb";

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _climbAction;

    private Rigidbody2D _rb;
    private ContactFilter2D _collisionFilter;
    private readonly RaycastHit2D[] _castHits = new RaycastHit2D[8];

    private float _moveInput;
    private float _climbInput;
    private Vector2 _velocity;
    private bool _isClimbing;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _ownsEnabledActions;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
            _climbAction?.Enable();
            _ownsEnabledActions = true;
        }
    }

    private void OnDisable()
    {
        if (_ownsEnabledActions)
        {
            _moveAction.Disable();
            _jumpAction.Disable();
            _climbAction?.Disable();
            _ownsEnabledActions = false;
        }
    }

    private void Update()
    {
        _moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>().x : 0f;
        _climbInput = ReadClimbInput();

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
        bool grounded = IsGrounded();
        bool pressingIntoWall = IsPressingIntoWall();
        _isClimbing = !grounded && pressingIntoWall && Mathf.Abs(_climbInput) > 0.01f;

        if (grounded)
        {
            _coyoteTimer = coyoteTime;
            if (_velocity.y < 0f)
            {
                _velocity.y = 0f;
            }
        }

        if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
        {
            _velocity.y = jumpSpeed;
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            _isClimbing = false;
        }

        _velocity.x = _moveInput * moveSpeed;
        if (_isClimbing)
        {
            _velocity.y = _climbInput * climbSpeed;
        }
        else
        {
            _velocity.y = Mathf.Max(_velocity.y - (gravity * dt), -maxFallSpeed);
        }

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

    private bool IsPressingIntoWall()
    {
        if (Mathf.Abs(_moveInput) < 0.01f)
        {
            return false;
        }

        float direction = Mathf.Sign(_moveInput);
        int hitCount = _rb.Cast(new Vector2(direction, 0f), _collisionFilter, _castHits, wallProbeDistance);
        for (int i = 0; i < hitCount; i++)
        {
            if (Mathf.Abs(_castHits[i].normal.x) > WallNormalThreshold)
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
            _climbAction = playerInput.actions.FindAction(climbActionName);
        }

        if ((_moveAction == null || _jumpAction == null || _climbAction == null) && inputActions != null)
        {
            _moveAction ??= inputActions.FindAction(moveActionName);
            _jumpAction ??= inputActions.FindAction(jumpActionName);
            _climbAction ??= inputActions.FindAction(climbActionName);
        }

        if (_moveAction == null || _jumpAction == null || _climbAction == null)
        {
            _moveAction ??= InputSystem.actions?.FindAction(moveActionName);
            _jumpAction ??= InputSystem.actions?.FindAction(jumpActionName);
            _climbAction ??= InputSystem.actions?.FindAction(climbActionName);
        }

        if (_moveAction == null || _jumpAction == null)
        {
            Debug.LogWarning("PlayerPlatforming: Could not find Move/Jump actions. Assign PlayerInput or InputActionAsset in the inspector.", this);
        }

        if (_climbAction == null)
        {
            Debug.LogWarning("PlayerPlatforming: Could not find Climb action.", this);
        }
    }

    private float ReadClimbInput()
    {
        if (_climbAction == null)
        {
            return 0f;
        }

        // Some bindings are scalar keys/buttons (float), others are sticks/composites (Vector2).
        InputControl activeControl = _climbAction.activeControl;
        if (activeControl != null && activeControl.valueType == typeof(Vector2))
        {
            Vector2 climbVector = _climbAction.ReadValue<Vector2>();
            return Mathf.Clamp(climbVector.y, -1f, 1f);
        }

        if (activeControl != null && activeControl.valueType == typeof(float))
        {
            float axis = _climbAction.ReadValue<float>();
            return Mathf.Clamp(axis, -1f, 1f);
        }

        // Fallback for controls that don't report a value type as expected.
        return _climbAction.IsPressed() ? 1f : 0f;
    }
}