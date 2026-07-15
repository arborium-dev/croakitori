// PlayerPlatforming.cs
// This script controls how the player physics and collisions function
// this also includes thr grapple physics, sadly

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

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

    [Header("Grapple Logic")]
    [SerializeField] private string grappleActionName = "Grapple";

    [SerializeField] private float grappleMaxDistance = 12f;

    [SerializeField] private float grapplePullSpeed = 25f;

    // Multiplies normal jump speed if you jump out of a grapple, might remove this tbh because its kind of level design breaking
    [SerializeField] private float grappleJumpBoost = 1.25f;
    
    [Header("Respawn UI")]
    [SerializeField] private Image fadeImage;
    [Tooltip("How fast the screen fades. Higher is faster.")]
    [SerializeField] private float fadeSpeed = 5f; 
    
    // This prevents the player from triggering multiple respawns at the same time
    private bool _isRespawning = false;
    
    // ANIMATION CODE YIPPEE

    [Header("Animation")] 
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private string _currentAnimState;
    
    // these must match the shits in unity
    private const string ANIM_IDLE = "idle";
    private const string ANIM_MOVE = "walkingRight";
    private const string ANIM_JUMP = "jumpingRight";
    
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _grappleAction;

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
    
    // --- GRAPPLE STATES ---
    private bool _isGrappleHeld;
    private bool _wasGrappleHeld;
    private bool _canGrapple = true; 
    private bool _isGrappling; 
    private bool _isGrappleMissed; 
    private bool _isGrappleRetracting; 
    private float _grappleMissDistance;
    private Vector2 _grapplePoint;
    private float _facingDir = 1f;
    private LineRenderer _grappleLine;
    private float _grappleReleaseTimer;
    
    public delegate void PlayerDeathDelegate();
    public event PlayerDeathDelegate OnPlayerDeath;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _collisionFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = groundLayer,
            useTriggers = false
        };
        
        
        // Set the initial checkpoint to where the player starts the level
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(transform.position);
        }

        ResolveActions();
    }

    private void OnEnable()
    {
        ResolveActions();

        if (playerInput == null && _moveAction != null && _jumpAction != null)
        {
            _moveAction.Enable();
            _jumpAction.Enable();
            if (_grappleAction != null) _grappleAction.Enable();
            _ownsEnabledActions = true;
        }
    }

    private void OnDisable()
    {
        if (_ownsEnabledActions)
        {
            _moveAction.Disable();
            _jumpAction.Disable();
            if (_grappleAction != null) _grappleAction.Disable();
            _ownsEnabledActions = false;
        }
    }

    private void Update()
    {
        _moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>().x : 0f;
        _isJumpHeld = _jumpAction != null && _jumpAction.IsPressed();
        _isGrappleHeld = _grappleAction != null && _grappleAction.IsPressed(); 
        
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

        HandleGrapple(dt);
        
        if (IsTouchingObject("Hazard"))
        {
            Respawn();
        }

        // if (IsTouchingObject("Checkpoint"))
        // {
        //     Debug.Log("Checkpoint");
        //     Debug.Log($"Checkpoint transform: {transform.position}");
        //     CheckpointManager.Instance.SetCheckpoint(transform.position);
        // }
        // handled in OnTriggerEnter, dont need predictive logic for checkpoints

        // Suspend standard physics if we are pulling OR if we are doing the miss animation
        if (!_isGrappling && !_isGrappleMissed)
        {
            if (IsGrounded())
            {
                _coyoteTimer = coyoteTime;
                _canGrapple = true; // RESET GRAPPLE CHARGE
                
                if (_velocity.y < 0f)
                {
                    _velocity.y = 0f;
                    _isJumping = false; // reset jumping state when grounded
                }
            }

            // initializing the jump
            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                _velocity.y = jumpSpeed;
                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f;
                _isJumping = true; 
            }

            // --- VARIABLE JUMP HEIGHT ---
            if (_isJumping && !_isJumpHeld && _velocity.y > 0f)
            {
                _velocity.y *= jumpCutMultiplier;
                _isJumping = false;
            }

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
        }

        Vector2 delta = _velocity * dt;
        MoveHorizontal(ref delta.x);
        MoveVertical(ref delta.y);

        _rb.MovePosition(_rb.position + delta);
        
        // anims!

        UpdateAnimation();

    }

    private void ChangeAnimationState(string newState)
    {
        if (_animator == null) return;
        
        // stop animator interupting itself
        if (_currentAnimState == newState) return;
        
        _animator.Play(newState);
        
        // update current state
        _currentAnimState = newState;
    }

    private void UpdateAnimation()
    {
        if (_spriteRenderer == null) return;
        
        // handle flipping
        if (_moveInput > 0.01f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_moveInput < -0.01f)
        {
            _spriteRenderer.flipX = true;
        }
        
        // handle state changes
        if (!IsGrounded())
        {
            ChangeAnimationState(ANIM_JUMP);
        }
        else if (Mathf.Abs(_moveInput) > 0.01f)
        {
            ChangeAnimationState(ANIM_MOVE);
        }
        else
        {
            ChangeAnimationState(ANIM_IDLE);
        }
    }

    private void HandleGrapple(float dt)
    {
        // keep track of facing direction
        if (Mathf.Abs(_moveInput) > 0.01f)
        {
            _facingDir = Mathf.Sign(_moveInput);
        }
        
        if (_grappleLine == null)
        {
            GameObject lrObj = new GameObject("GrappleLine");
            lrObj.transform.SetParent(transform);
            _grappleLine = lrObj.AddComponent<LineRenderer>();
            _grappleLine.startWidth = 0.15f;
            _grappleLine.endWidth = 0.15f;
            _grappleLine.material = new Material(Shader.Find("Sprites/Default"));
            _grappleLine.startColor = Color.red;
            _grappleLine.endColor = Color.red;
            _grappleLine.positionCount = 2;
            _grappleLine.sortingOrder = 10;
            _grappleLine.useWorldSpace = true;
            _grappleLine.enabled = false;
        }

        Vector2 origin = _collider.bounds.center; // shoot from player
        bool grapplePressedThisFrame = _isGrappleHeld && !_wasGrappleHeld;
        
        // 1. INIT SHOT LOGIC
        if (grapplePressedThisFrame && _canGrapple && !_isGrappling && !_isGrappleMissed)
        {
            _canGrapple = false; // Consume the charge
            
            Vector2 castDir = new Vector2(_facingDir, 0f);
            int hits = Physics2D.Raycast(origin, castDir, _collisionFilter, _castHits, grappleMaxDistance);
            bool hitWall = false;

            for (int i = 0; i < hits; i++) 
            {
                if (_castHits[i].collider.attachedRigidbody == _rb) continue;

                if (Mathf.Abs(_castHits[i].normal.x) > WallNormalThreshold) 
                {
                    _grapplePoint = _castHits[i].point;
                    _isGrappling = true;
                    hitWall = true;
                    break;
                }
            }

            // Trigger Miss state if we didn't find a wall
            if (!hitWall)
            {
                _isGrappleMissed = true;
                _isGrappleRetracting = false;
                _grappleMissDistance = 0f;
            }
        }
        
        // 2. STATE BEHAVIORS
        if (_isGrappling)
        {
            if (!_isGrappleHeld || _jumpBufferTimer > 0f) // cancel grapple
            {
                _isGrappling = false;
                _grappleLine.enabled = false;
                
                _coyoteTimer = coyoteTime;
                _grappleReleaseTimer = 0.15f; 
            }
            else 
            {
                float pullDir = Mathf.Sign(_grapplePoint.x - _rb.position.x);
                float dist = Mathf.Abs(_grapplePoint.x - _rb.position.x);
                
                if (dist > skinWidth * 3f) 
                {
                    _velocity.x = pullDir * grapplePullSpeed;
                    _velocity.y = 0f;
                }
                else 
                {
                    _velocity.x = 0f;
                    _velocity.y = 0f;
                }

                _grappleLine.enabled = true;
                _grappleLine.SetPosition(0, new Vector3(origin.x, origin.y, 0f));
                _grappleLine.SetPosition(1, new Vector3(_grapplePoint.x, _grapplePoint.y, 0f));
            }
        }
        else if (_isGrappleMissed)
        {
            // LOCK velocity mid-air so player stops perfectly
            _velocity = Vector2.zero; 
            
            // Double speed so the miss feels snappy like Celeste
            float missAnimSpeed = grapplePullSpeed * 2f; 

            if (!_isGrappleRetracting)
            {
                // Extending outwards
                _grappleMissDistance += missAnimSpeed * dt;
                if (_grappleMissDistance >= grappleMaxDistance)
                {
                    _grappleMissDistance = grappleMaxDistance;
                    _isGrappleRetracting = true;
                }
            }
            else
            {
                // Retracting inwards
                _grappleMissDistance -= missAnimSpeed * dt;
                if (_grappleMissDistance <= 0f)
                {
                    // Animation complete, Resume falling
                    _isGrappleMissed = false;
                    _grappleLine.enabled = false;
                }
            }

            // Draw line dynamically while missing
            if (_isGrappleMissed) 
            {
                _grappleLine.enabled = true;
                Vector3 startPos = new Vector3(origin.x, origin.y, 0f);
                Vector3 endPos = startPos + new Vector3(_facingDir * _grappleMissDistance, 0f, 0f);
                
                _grappleLine.SetPosition(0, startPos);
                _grappleLine.SetPosition(1, endPos);
            }
        }
        else // Not grappling and not missing
        {
            _grappleLine.enabled = false;
            
            if (_grappleReleaseTimer > 0f)
            {
                _grappleReleaseTimer -= dt;
                
                if (_isJumping)
                {
                    _velocity.y = jumpSpeed * grappleJumpBoost;
                    _grappleReleaseTimer = 0f; // Consume boost
                }
            }
        }
        
        _wasGrappleHeld = _isGrappleHeld;
    } // what a long function am i right guys?
    
    
    
    // WARNING 
    // ALL CODE BENEATH THIS IS FROM EARLY DEVELOPMENT
    // I DONT REALLY KNOW WHAT MOST OF IT DOES (EXCEPT FOR THE HAZARD LOGIC)
    // ALL I KNOW IS THAT IF I CHANGE ANY OF IT, IT BREAKS
    // theres some predictive shit as well
    // todo: refactor

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

    public bool IsGrounded() // the ingredients also use this!
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
            _grappleAction = playerInput.actions.FindAction(grappleActionName);
        }

        if ((_moveAction == null || _jumpAction == null || _grappleAction == null) && inputActions != null)
        {
            _moveAction ??= inputActions.FindAction(moveActionName);
            _jumpAction ??= inputActions.FindAction(jumpActionName);
            _grappleAction  ??= inputActions.FindAction(grappleActionName);
        }

        if (_moveAction == null || _jumpAction == null || _grappleAction == null)
        {
            _moveAction ??= InputSystem.actions?.FindAction(moveActionName);
            _jumpAction ??= InputSystem.actions?.FindAction(jumpActionName);
            _grappleAction  ??= InputSystem.actions?.FindAction(grappleActionName);
        }

        if (_moveAction == null || _jumpAction == null || _grappleAction == null)
        {
            Debug.LogWarning("PlayerPlatforming: Could not find Move/Jump/Grapple actions. Assign PlayerInput or InputActionAsset in the inspector.", this);
        }
    }
    

    private bool IsTouchingObject(string tagName) // only works for colliable objects
    {
        Vector2[] directions = { Vector2.down, Vector2.up, Vector2.left, Vector2.right };

        foreach (Vector2 direction in directions)
        {
            int hitCount = _rb.Cast(direction, _collisionFilter, _castHits, groundProbeDistance);

            for (int i = 0; i < hitCount; i++)
            {
                if (_castHits[i].collider.CompareTag(tagName))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint");
            Debug.Log($"Checkpoint transform: {collision.transform.position}");
            CheckpointManager.Instance.SetCheckpoint(transform.position);
        }    
        
    }

    private void Respawn()
    {
        // Prevent starting the sequence multiple times if touching multiple hazards
        if (_isRespawning) return;
        
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        _isRespawning = true;

        // 1. FADE TO BLACK
        if (fadeImage != null)
        {
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                // Keep the color black, just change the alpha
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null; // Wait until next frame
            }
        }

        // 2. DO THE ACTUAL RESPAWN LOGIC (While the screen is fully black)
        OnPlayerDeath?.Invoke();
    
        // teleport
        transform.position = CheckpointManager.Instance.currentCheckpoint;
    
        // reset momentum
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    
        // reset grapple states
        _isGrappling = false;
 
        // Wait a tiny fraction of a second in the black screen before fading back
        yield return new WaitForSeconds(0.1f);

        // 3. FADE BACK TO CLEAR
        if (fadeImage != null)
        {
            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                fadeImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null; // Wait until next frame
            }
        }

        _isRespawning = false;
    }
}