using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UltimatePlayerController : MonoBehaviour
{
    #region 1. CONFIGURATION

    [Header("1. Movement Settings")]
    [SerializeField] private float _walkSpeed = 8f;
    [SerializeField] private float _runSpeed = 14f;
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

    #endregion

    #region 2. STATE VARIABLES

    private Rigidbody2D _rb;
    private Vector2 _input;

    private bool _isGrounded;
    private bool _isFacingRight = true;
    private bool _isTouchingWall;
    private bool _isWallSliding;
    private bool _isRunning;

    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.gravityScale = _gravityScale;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        _input.x = Input.GetAxisRaw("Horizontal");
        _isRunning = Input.GetKey(KeyCode.LeftShift) && _input.x != 0;

        if (Input.GetButtonDown("Jump"))
            _jumpBufferCounter = _jumpBufferTime;
        else
            _jumpBufferCounter -= Time.deltaTime;

        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0)
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _jumpCutMultiplier);

        HandleAnimation();
        CheckFlip();
    }

    private void FixedUpdate()
    {
        RunCollisionChecks();

        if (_isGrounded) _coyoteTimeCounter = _coyoteTime;
        else _coyoteTimeCounter -= Time.fixedDeltaTime;

        Move();
        ApplyGravityModifier();
        HandleWallSlide();

        if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0 && !_isWallSliding)
            Jump();
    }

    #region MOVEMENT

    private void Move()
    {
        float currentSpeed = _isRunning ? _runSpeed : _walkSpeed;

        float targetSpeed = _input.x * currentSpeed;
        float speedDif = targetSpeed - _rb.velocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? _acceleration : _deceleration;
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
        if (_rb.velocity.y < 0)
            _rb.gravityScale = _gravityScale * _fallGravityMult;
        else
            _rb.gravityScale = _gravityScale;
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

    #endregion

    #region COLLISION

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
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;
        }
    }

    #endregion

    #region ANIMATION (SpriteAnimator)

    private void HandleAnimation()
    {
        // WALL SLIDE
        /*if (_isWallSliding)
        {
            _spriteAnimator.Play(AnimState.Flip);
            return;
        }

        // JUMP
      

        // FALL
        /*if (!_isGrounded && _rb.velocity.y < -0.1f)
        {
            _spriteAnimator.Play(AnimState.Hurt); // hoặc Fall nếu bạn có
            return;
        }*/

        // IDLE
       
    }

    #endregion

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
