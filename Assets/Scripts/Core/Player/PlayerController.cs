using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Clone Setting")]
    public bool IsClone = false;
    public PlayerInputData cloneInput;
    [Header("Run Stop Delay")]
    public float stopRunDelay = 0.1f;
    private float stopRunTimer = 0f;
    private bool wasRunningPrevFrame = false;

    private float movementInputDirection;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool isWalkingOrRunning;
    private bool isRunning;
    private bool _isDeath;

    public bool IsDeath
    {
        get { return _isDeath; }
        set
        {
            _isDeath = value;
            if (_isDeath)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
                anim.SetBool("Death", _isDeath);
                Death();
            }
        }
    }

    private bool canFlip = true;

    private bool justWallJumped = false;
    private float autoFlipTimer = 0f;
    private float autoFlipDelay = 0.05f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private InputRecorder _inputRecorder;
    public int amountOfJumps = 1;

    public float runSpeed = 7f;
    public float movementSpeed = 5f;
    public float jumpForce = 12f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float wallHopForce;
    public float wallJumpForce;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    void Start()
    {
        amountOfJumpsLeft = amountOfJumps;

        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        if (_isDeath) return;
        
        HandleRunStopDelay();
        HandleDashTimers();
        HandleAutoFlipAfterWallJump();

        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        HandleWallDrop();
        CheckIfWallSliding();
    }

    private void FixedUpdate()
    {
        if (_isDeath) return;

        CheckSurroundings();
        ApplyMovement();
    }


    private void Death()
    {
        if (IsClone)
        {
            CloneManager.Instance.RemoveCloneExact(gameObject);
        }
    }

    public void ResetDeath()
    {
        if (IsClone)
        {
            CloneManager.Instance.DestroyCloneDeath(gameObject);
        }
        else
        {
            _isDeath = false;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;

            transform.position = _inputRecorder.OriginPos;
            anim.SetBool("Death", false);

            isDashing = false;
            dashTimer = 0f;
            dashCooldownTimer = 0f;
            canFlip = true;
        }
    }




    private void HandleWallDrop()
    {
        float verticalInput = IsClone ? cloneInput.vertical : Input.GetAxisRaw("Vertical");

        if (isWallSliding && verticalInput < 0)
        {
            isWallSliding = false;

            Vector2 force = new Vector2(
                -facingDirection * 0.1f,
                wallHopForce * -Mathf.Abs(wallHopDirection.y)
            );

            rb.AddForce(force, ForceMode2D.Impulse);


            justWallJumped = false;
        }
    }



    private void HandleRunStopDelay()
    {
        bool isCurrentlyMoving = Mathf.Abs(movementInputDirection) > 0.1f;

        if (wasRunningPrevFrame && !isRunning && isCurrentlyMoving)
        {
            stopRunTimer = stopRunDelay;
        }

        if (stopRunTimer > 0)
            stopRunTimer -= Time.deltaTime;

        wasRunningPrevFrame = isRunning;
    }



    private void CheckInput()
    {
        if (isDashing) return;

        if (!IsClone)
        {
            movementInputDirection = Input.GetAxisRaw("Horizontal");
            isRunning = Input.GetKey(KeyCode.LeftShift);
            if (Input.GetButtonDown("Jump"))
                Jump();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                TryDash();
        }
        else
        {
            movementInputDirection = cloneInput.horizontal;
            isRunning = cloneInput.runHeld;
            if (cloneInput.jumpPressed)
                Jump();

            if (cloneInput.dashPressed)
                TryDash();
        }
    }

    private void TryDash()
    {
        if (!isGrounded) return;           
        if (isDashing) return;
        if (dashCooldownTimer > 0) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        canFlip = false;

        rb.velocity = new Vector2(facingDirection * dashSpeed, 0);
    }



    private void HandleDashTimers()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        rb.velocity = new Vector2(facingDirection * dashSpeed, 0);

        if (isTouchingWall)
        {
            isDashing = false;
            canFlip = true;
            justWallJumped = false;
            return;
        }

        if (dashTimer <= 0)
        {
            isDashing = false;
            canFlip = true;
        }
    }
    private void Jump()
    {
        if (isDashing) return;

        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
        else if (isWallSliding && movementInputDirection == 0 && canJump)
        {
            isWallSliding = false;
            amountOfJumpsLeft--;

            Vector2 force = new Vector2(
                wallHopForce * wallHopDirection.x * -facingDirection,
                wallHopForce * wallHopDirection.y
            );

            rb.AddForce(force, ForceMode2D.Impulse);

            justWallJumped = true;
            autoFlipTimer = autoFlipDelay;
        }
        else if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump)
        {
            isWallSliding = false;
            amountOfJumpsLeft--;

            Vector2 force = new Vector2(
                wallJumpForce * wallJumpDirection.x * movementInputDirection,
                wallJumpForce * wallJumpDirection.y
            );

            rb.AddForce(force, ForceMode2D.Impulse);

            justWallJumped = true;
            autoFlipTimer = autoFlipDelay;
        }
    }
    private void HandleAutoFlipAfterWallJump()
    {
        if (!justWallJumped) return;
        if (isDashing) return;

        autoFlipTimer -= Time.deltaTime;

        if (autoFlipTimer <= 0)
        {
            int newDir = rb.velocity.x > 0 ? 1 : -1;

            if (newDir != facingDirection)
            {
                facingDirection = newDir;
                isFacingRight = newDir == 1;
                transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
            }

            justWallJumped = false;
        }
    }

    private void CheckMovementDirection()
    {
        if (!canFlip || justWallJumped || isDashing) return;

        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        isWalkingOrRunning = rb.velocity.x != 0 && movementInputDirection != 0;
    }

    private void Flip()
    {
        if (!canFlip || isWallSliding || justWallJumped || isDashing) return;

        facingDirection *= -1;
        isFacingRight = !isFacingRight;

        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
    }

    private void CheckIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
            amountOfJumpsLeft = amountOfJumps;

        canJump = amountOfJumpsLeft > 0 && (isGrounded || isWallSliding);
    }

    private void CheckIfWallSliding()
    {
        if (isDashing) return;

        if (justWallJumped)
        {
            isWallSliding = isTouchingWall && rb.velocity.y < 0 && !isGrounded;
            return;
        }

        isWallSliding = isTouchingWall && !isGrounded && rb.velocity.y < 0;
    }

    private void CheckSurroundings()
    {
        isGrounded =
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall =
            Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    // ============================================================
    // APPLY MOVEMENT
    // ============================================================
    private void ApplyMovement()
    {
        if (isDashing) return; // dash override toàn bộ movement

        if (isGrounded)
        {
            float targetSpeed;

            if (stopRunTimer > 0)
            {
                targetSpeed = runSpeed;
            }
            else
            {
                targetSpeed = (isRunning ? runSpeed : movementSpeed);
            }

            rb.velocity = new Vector2(
                targetSpeed * movementInputDirection,
                rb.velocity.y
            );
        }

        else if (!isWallSliding && movementInputDirection != 0)
        {
            rb.AddForce(new Vector2(movementForceInAir * movementInputDirection, 0));

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        else if (!isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

        if (isWallSliding && rb.velocity.y < -wallSlideSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
    }

    // ============================================================
    // ANIMATIONS
    // ============================================================
    private void UpdateAnimations()
    {
        anim.SetFloat("Speed", !isWalkingOrRunning ? 0 : isRunning ? 1.5f : 0.5f);
        anim.SetBool("IsWalkOrRun", isWalkingOrRunning);
        anim.SetBool("IsGounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("IsWallSliding", isWallSliding);
        anim.SetBool("IsDashing", isDashing);

        spriteRenderer.flipX = isWallSliding;
    }

    // ============================================================
    // GIZMOS
    // ============================================================
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * wallCheckDistance);
    }
}
