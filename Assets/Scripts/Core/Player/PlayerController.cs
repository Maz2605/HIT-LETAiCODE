using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3;
    public float runSpeed = 5;
    public float jumpForce = 6;
    public float dashPower = 12;
    public float dashTime = 0.15f;
    public LayerMask groundLayer;
    public Transform groundCheck; // Tạo empty GameObject làm con, đặt ở chân player

    Rigidbody2D rb;
    PlayerStateMachine sm;

    bool isDashing;
    float dashTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sm = GetComponent<PlayerStateMachine>();
    }

    void Update()
    {
        if (sm.Current == PlayerState.Die) return;

        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        sm.IsGrounded = hit != null;
        // Xử lý Dash
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
            else
            {
                return; 
            }
        }

        float h = Input.GetAxisRaw("Horizontal");

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing)
        {
            if (sm.TryChange(PlayerState.Dash))
            {
                isDashing = true;
                dashTimer = dashTime;
                rb.velocity = new Vector2(transform.localScale.x * dashPower, 0);
            }
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sm.TryChange(PlayerState.Jump))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // Movement (air control - hoạt động cả khi nhảy!)
        if (!isDashing)
        {
            if (Mathf.Abs(h) > 0.1f)
            {
                bool running = Input.GetKey(KeyCode.LeftShift);
                float spd = running ? runSpeed : walkSpeed;
                rb.velocity = new Vector2(h * spd, rb.velocity.y);

                // Chỉ update state khi đang ở trên mặt đất
                if (sm.IsGrounded)
                {
                    sm.TryChange(running ? PlayerState.Run : PlayerState.Walk);
                }
            }
            else if (sm.IsGrounded)
            {
                sm.TryChange(PlayerState.Idle);
            }
        }

        // Tự động chuyển về Jump state khi rời mặt đất (rơi xuống)
        if (!sm.IsGrounded && sm.Current != PlayerState.Dash && sm.Current != PlayerState.Jump)
        {
            sm.Change(PlayerState.Jump); // Falling cũng dùng Jump state
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        // Animation sẽ dựa vào state và IsGrounded
        // anim.SetInteger("State", (int)sm.Current);
        // anim.SetBool("IsGrounded", sm.IsGrounded);
    }
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }

}