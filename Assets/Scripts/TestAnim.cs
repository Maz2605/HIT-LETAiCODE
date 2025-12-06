using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UltimatePlayerController : MonoBehaviour
{
    #region 1. CONFIGURATION

    [Header("1. Movement Settings")]
    [SerializeField] private float _walkSpeed = 8f;   // Tốc độ đi bộ (Mặc định)
    [SerializeField] private float _runSpeed = 14f;   // Tốc độ chạy (Khi giữ Shift)
    [SerializeField] private float _acceleration = 70f;
    [SerializeField] private float _deceleration = 60f;
    [SerializeField] private float _velPower = 0.95f;

    [Header("2. Jump Settings")]
    [SerializeField] private float _jumpForce = 16f;
    [SerializeField] private float _jumpCutMultiplier = 0.5f;
    [SerializeField] private float _fallGravityMult = 1.8f;
    [SerializeField] private float _gravityScale = 3f;
    [SerializeField] private float _coyoteTime = 0.15f;
    [SerializeField] private float _jumpBufferTime = 0.1f;

    [Header("3. Physics Checks")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask _groundLayer;

    [Header("3b. Wall Checks")]
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckRadius = 0.4f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallSlideSpeed = 2f;

    [Header("4. Animation Settings")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _maxVerticalSpeed = 18f;
    [SerializeField] private float _animDampTime = 0.05f;

    #endregion

    #region 2. STATE VARIABLES

    private Rigidbody2D _rb;
    private Vector2 _input;

    // States
    private bool _isGrounded;
    private bool _isFacingRight = true;
    private bool _isTouchingWall;
    private bool _isWallSliding;
    private bool _isRunning; // Trạng thái chạy

    // Timers
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    // Hashing IDs
    private readonly int _yVelocityHash = Animator.StringToHash("yVelocity");
    private readonly int _xVelocityHash = Animator.StringToHash("xVelocity");
    private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int _isWallSlidingHash = Animator.StringToHash("IsWallSliding");

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();

        _rb.gravityScale = _gravityScale;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        // 1. Nhận Input
        _input.x = Input.GetAxisRaw("Horizontal");

        // --- MỚI: Check Input Run ---
        // Chỉ chạy khi giữ Shift VÀ đang di chuyển
        _isRunning = Input.GetKey(KeyCode.LeftShift) && _input.x != 0;

        // 2. Jump Buffer
        if (Input.GetButtonDown("Jump")) _jumpBufferCounter = _jumpBufferTime;
        else _jumpBufferCounter -= Time.deltaTime;

        // 3. Variable Jump
        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _jumpCutMultiplier);
        }

        // 4. Update Animation
        HandleAnimation();

        // 5. Flip Character
        CheckFlip();
    }

    private void FixedUpdate()
    {
        // 1. Check Collisions
        RunCollisionChecks();

        // 2. Coyote Time
        if (_isGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        // 3. Di chuyển (Logic Run nằm trong hàm này)
        Move();

        // 4. Gravity
        ApplyGravityModifier();

        // 5. Wall Slide
        HandleWallSlide();

        // 6. Execute Jump
        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f && !_isWallSliding)
        {
            Jump();
        }
    }

    // --- Core Logic ---

    private void Move()
    {
        // --- MỚI: Chọn tốc độ dựa trên việc có giữ Shift hay không ---
        float currentTargetSpeed = _isRunning ? _runSpeed : _walkSpeed;

        // Tính toán lực đẩy
        float targetSpeed = _input.x * currentTargetSpeed;
        float speedDif = targetSpeed - _rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, _velPower) * Mathf.Sign(speedDif);

        _rb.AddForce(movement * Vector2.right);
    }

    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

        _jumpBufferCounter = 0;
        _coyoteTimeCounter = 0;
    }

    private void ApplyGravityModifier()
    {
        if (_rb.velocity.y < 0) _rb.gravityScale = _gravityScale * _fallGravityMult;
        else _rb.gravityScale = _gravityScale;
    }

    private void HandleWallSlide()
    {
        if (_isTouchingWall && !_isGrounded && _rb.velocity.y < 0 && _input.x != 0)
        {
            _isWallSliding = true;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -_wallSlideSpeed, float.MaxValue));
        }
        else
        {
            _isWallSliding = false;
        }
    }

    private void RunCollisionChecks()
    {
        _isGrounded = Physics2D.BoxCast(_groundCheck.position, _groundCheckSize, 0f, Vector2.down, 0.05f, _groundLayer);
        _isTouchingWall = Physics2D.OverlapCircle(_wallCheck.position, _wallCheckRadius, _wallLayer);
    }

    private void CheckFlip()
    {
        if (_isWallSliding) return;

        if ((_isFacingRight && _input.x < 0) || (!_isFacingRight && _input.x > 0))
        {
            _isFacingRight = !_isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // --- Animation Logic ---

    private void HandleAnimation()
    {
        _animator.SetBool(_isGroundedHash, _isGrounded);
        _animator.SetBool(_isWallSlidingHash, _isWallSliding);

        // --- MỚI: Logic Normalize xVelocity cho Run/Walk ---
        // Chúng ta chia cho RunSpeed (Tốc độ lớn nhất) để chuẩn hóa về 0 -> 1
        // Nếu đi bộ: 8 / 14 = ~0.57 -> Blend Tree chạy Walk
        // Nếu chạy: 14 / 14 = 1.0 -> Blend Tree chạy Run
        float normalizedX = Mathf.Abs(_rb.velocity.x) / _runSpeed;

        _animator.SetFloat(_xVelocityHash, normalizedX, _animDampTime, Time.deltaTime);

        // yVelocity Logic
        float normalizedY = _rb.velocity.y / _maxVerticalSpeed;
        normalizedY = Mathf.Clamp(normalizedY, -1f, 1f);
        _animator.SetFloat(_yVelocityHash, normalizedY, _animDampTime, Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }

        if (_wallCheck != null)
        {
            Gizmos.color = _isTouchingWall ? Color.blue : Color.yellow;
            Gizmos.DrawWireSphere(_wallCheck.position, _wallCheckRadius);
        }
    }
}   