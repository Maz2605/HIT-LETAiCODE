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

    private bool justWallJumped = false;
    private float autoFlipTimer = 0f;
    private float autoFlipDelay = 0.05f;

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
    public Transform ledgeCheck;

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

    // ============================================
    // INPUT
    // ============================================
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            Jump();

        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonUp("Jump"))
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
    }

    // ============================================
    // JUMP / WALLJUMP
    // ============================================
    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
        else if (isWallSliding && movementInputDirection == 0 && canJump)
        {
            // WALL HOP
            isWallSliding = false;
            amountOfJumpsLeft--;

            // dùng lực nhảy bật ra
            Vector2 force = new Vector2(
                wallHopForce * wallHopDirection.x * -facingDirection,
                wallHopForce * wallHopDirection.y
            );

            rb.AddForce(force, ForceMode2D.Impulse);

            // chuẩn bị auto flip
            justWallJumped = true;
            autoFlipTimer = autoFlipDelay;
        }
        else if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump)
        {
            // WALL JUMP ĐÚNG HƯỚNG INPUT
            isWallSliding = false;
            amountOfJumpsLeft--;

            Vector2 force = new Vector2(
                wallJumpForce * wallJumpDirection.x * movementInputDirection,
                wallJumpForce * wallJumpDirection.y
            );

            rb.AddForce(force, ForceMode2D.Impulse);

            // chuẩn bị auto flip
            justWallJumped = true;
            autoFlipTimer = autoFlipDelay;
        }
    }

    // ============================================
    // AUTO FLIP SAU WALL JUMP
    // ============================================
    private void HandleAutoFlipAfterWallJump()
    {
        if (!justWallJumped) return;

        autoFlipTimer -= Time.deltaTime;

        if (autoFlipTimer <= 0)
        {
            int newDir = rb.velocity.x > 0 ? 1 : -1;

            if (newDir != facingDirection)
            {
                facingDirection = newDir;
                isFacingRight = newDir == 1;
                transform.rotation = Quaternion.Euler(0, newDir == 1 ? 0 : 180, 0);
            }

            justWallJumped = false;
        }
    }

    // ============================================
    // MOVEMENT / FLIP
    // ============================================
    private void CheckMovementDirection()
    {
        if (!canFlip || justWallJumped) return;

        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        isWalkingOrRunning = rb.velocity.x != 0;
    }

    private void Flip()
    {
        if (!canFlip) return;
        if (isWallSliding) return;
        if (justWallJumped) return;

        facingDirection *= -1;
        isFacingRight = !isFacingRight;

        transform.rotation =
            Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
    }

    // ============================================
    // CHECKS
    // ============================================
    private void CheckIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
            amountOfJumpsLeft = amountOfJumps;

        canJump = amountOfJumpsLeft > 0;
    }

    private void CheckIfWallSliding()
    {
        // nếu vừa wall jump → được phép dính tường ngay khi chạm tường mới
        if (justWallJumped)
        {
            isWallSliding = isTouchingWall && rb.velocity.y < 0 && !isGrounded;
            return;
        }

        isWallSliding =
            isTouchingWall &&
            !isGrounded &&
            rb.velocity.y < 0;
    }

    private void CheckSurroundings()
    {
        isGrounded =
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall =
            Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    // ============================================
    // PHYSICS
    // ============================================
    private void ApplyMovement()
    {
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

    // ============================================
    // ANIMATION
    // ============================================
    private void UpdateAnimations()
    {
        anim.SetFloat("Speed", !isWalkingOrRunning ? 0 : isRunning ? 1.5f : 0.5f);
        anim.SetBool("IsWalkOrRun", isWalkingOrRunning);
        anim.SetBool("IsGounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("IsWallSliding", isWallSliding);

        spriteRenderer.flipX = isWallSliding;
    }

    // ============================================
    // GIZMOS
    // ============================================
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * wallCheckDistance);
    }
}
