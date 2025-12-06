using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3;
    public float runSpeed = 5;
    public float jumpForce = 6;
    public float dashPower = 12;
    public float dashTime = 0.15f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    Rigidbody2D rb;
    PlayerStateMachine sm;
    Animator anim;

    float inputDir = 0;  
    float moveDir = 0;   
    bool isDashing;
    float dashTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sm = GetComponent<PlayerStateMachine>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (sm.Current == PlayerState.Die) return;

        CheckGrounded();
        HandleDash();
        HandleJump();
        HandleMovement();
        HandleAirState();
        UpdateAnimation();
    }

    void CheckGrounded()
    {
        sm.IsGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    void HandleDash()
    {
        if (isDashing)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashPower, 0);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                sm.Change(sm.IsGrounded ? PlayerState.Idle : PlayerState.Fall);
                rb.velocity = Vector2.zero;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (sm.TryChange(PlayerState.Dash))
            {
                isDashing = true;
                dashTimer = dashTime;
                rb.velocity = new Vector2(transform.localScale.x * dashPower, 0);
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sm.TryChange(PlayerState.Jump))
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleMovement()
    {
        if (isDashing) return;

        inputDir = Input.GetAxisRaw("Horizontal");
        bool hasInput = Mathf.Abs(inputDir) > 0.1f;
        bool running = Input.GetKey(KeyCode.LeftShift);

        // ======================
        // 1) ĐỔI HƯỚNG → TURN
        // ======================
        if (sm.IsGrounded &&
            (sm.Current == PlayerState.Run || sm.Current == PlayerState.Walk) &&
            hasInput &&
            moveDir != 0 &&
            Mathf.Sign(inputDir) != Mathf.Sign(moveDir))
        {
            sm.Change(PlayerState.Turn);
            moveDir = inputDir;
            return;
        }

        // ======================
        // 2) ĐỨNG LẠI → RUN_TO_IDLE
        // ======================
        if (!hasInput)
        {
            if (moveDir != 0)    // vừa thả phím → đang chạy → chạy anim run_to_idle
            {
                sm.Change(PlayerState.RunToIdle);
                moveDir = 0;
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 12f * Time.deltaTime), rb.velocity.y);
                return;
            }

            // thực sự idle
            sm.Change(PlayerState.Idle);
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        // ======================
        // 3) DI CHUYỂN BÌNH THƯỜNG
        // ======================
        float spd = running ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(inputDir * spd, rb.velocity.y);

        transform.localScale = new Vector3(inputDir > 0 ? 1 : -1, 1, 1);

        if (sm.IsGrounded)
            sm.Change(running ? PlayerState.Run : PlayerState.Walk);

        moveDir = inputDir;
    }

    void HandleAirState()
    {
        if (!sm.IsGrounded && sm.Current != PlayerState.Jump && sm.Current != PlayerState.Dash)
            sm.Change(PlayerState.Jump);
    }

    void UpdateAnimation()
    {
        anim.SetInteger("State", (int)sm.Current);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }
}
