using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private bool canFlip = true;

    // --- WALLJUMP AUTO FLIP ---
    private bool justWallJumped = false;
    private float autoFlipTimer = 0f;
    private float autoFlipDelay = 0.05f;

    // --- DASH ---
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public int amountOfJumps = 1;

    public float runSpeed = 5f;
    public float movementSpeed = 3f;
    public float jumpForce = 6f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        amountOfJumpsLeft = amountOfJumps;

        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        HandleDashTimers();
        HandleAutoFlipAfterWallJump();

        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        ApplyMovement();
    }

    // ============================================================
    // INPUT
    // ============================================================
    private void CheckInput()
    {
        if (isDashing) return; // khóa input khi dash

        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            Jump();

        if (Input.GetButtonUp("Jump"))
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);

        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftControl))
            TryDash();
    }

    // ============================================================
    // DASH
    // ============================================================
    private void TryDash()
    {
        if (isDashing) return;
        if (dashCooldownTimer > 0) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // khóa control
        canFlip = false;

        // hướng dash dựa vào facingDirection
        rb.velocity = new Vector2(facingDirection * dashSpeed, 0);
    }

    private void HandleDashTimers()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        // đang dash → force velocity để không đổi hướng
        rb.velocity = new Vector2(facingDirection * dashSpeed, 0);

        // nếu đập tường → kết thúc dash ngay & bắt đầu wall slide
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

    // ============================================================
    // JUMP + WALLJUMP
    // ============================================================
    private void Jump()
    {
        if (isDashing) return; // dash thì ko cho nhảy

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

    // ============================================================
    // AUTO FLIP SAU WALL JUMP
    // ============================================================
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

    // ============================================================
    // MOVEMENT + FLIP
    // ============================================================
    private void CheckMovementDirection()
    {
        if (!canFlip || justWallJumped || isDashing) return;

        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        isWalkingOrRunning = rb.velocity.x != 0;
    }

    private void Flip()
    {
        if (!canFlip || isWallSliding || justWallJumped || isDashing) return;

        facingDirection *= -1;
        isFacingRight = !isFacingRight;

        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
    }

    // ============================================================
    // CHECKS
    // ============================================================
    private void CheckIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
            amountOfJumpsLeft = amountOfJumps;

        canJump = amountOfJumpsLeft > 0;
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
            rb.velocity = new Vector2(
                (isRunning ? runSpeed : movementSpeed) * movementInputDirection,
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
