using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Patrol, Chase, Attacking, Defending, Dead, Resetting }

    [Header("--- TRẠNG THÁI CHUNG ---")]
    public BossState currentState = BossState.Patrol;
    public float maxHealth = 500f;
    private float currentHealth;
    public bool isChasing = false;
    private Vector3 startPosition;

    [Header("--- ĐI TUẦN & TẦM NHÌN ---")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float visionRange = 10f;
    private Transform currentPatrolTarget; 

    [Header("--- PHẠM VI TẤN CÔNG (QUAN TRỌNG) ---")]
    public float meleeRange = 3f;   // Dưới 3m là Đấm
    public float shootRange = 12f;  // Dưới 12m là Bắn (Trên 12m là Đuổi)
    public Transform player;

    [Header("--- KỸ NĂNG 1: BẮN XA ---")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("--- KỸ NĂNG 2: CẬN CHIẾN ---")]
    public Transform meleePoint;
    public float meleeRadius = 1.5f;

    [Header("--- KỸ NĂNG 3: KHIÊN ---")]
    public GameObject shieldVisual; 
    public float shieldDuration = 2f; 
    public float shieldCooldown = 5f; 
    private float nextShieldTime = 0f;

    // Components
    private Animator anim;
    private Rigidbody2D rb;
    public float chaseSpeed = 4f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        startPosition = transform.position;
        currentPatrolTarget = pointA;
        
        if (shieldVisual) shieldVisual.SetActive(false);
        StartCoroutine(RunBossLogic());
    }

    void Update()
    {
        if (currentState == BossState.Dead) return;

        if (player != null)
        {
            if (isChasing) LookAtTarget(player.position);
            else if (currentState == BossState.Patrol && currentPatrolTarget != null) 
                LookAtTarget(currentPatrolTarget.position);
        }
        else if (currentState == BossState.Resetting)
        {
            LookAtTarget(startPosition);
        }
    }

    // =========================================================
    //               BỘ NÃO BOSS (LOGIC MỚI)
    // =========================================================
    IEnumerator RunBossLogic()
    {
        while (currentHealth > 0)
        {
            // 1. Nếu Player chết -> Về nhà
            if (player == null)
            {
                yield return StartCoroutine(ReturnToStartAndStop());
                continue; 
            }

            // 2. Logic ĐI TUẦN (Khi chưa phát hiện)
            if (!isChasing)
            {
                currentState = BossState.Patrol;
                if (CanSeePlayer())
                {
                    Debug.Log("BOSS: Phát hiện!");
                    isChasing = true;
                    if(anim) anim.Play("Idle"); 
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    PatrolBehavior();
                    yield return null;
                    continue; 
                }
            }

            // 3. Logic CHIẾN ĐẤU (Khi đã phát hiện)
            currentState = BossState.Chase;
            float distance = Vector2.Distance(transform.position, player.position);

            // --- PHÂN LOẠI KHOẢNG CÁCH RÕ RÀNG ---
            
            if (distance <= meleeRange)
            {
                // CASE 1: CỰC GẦN -> ĐẤM NGAY
                yield return StartCoroutine(PerformMeleeAttack());
            }
            else if (distance <= shootRange)
            {
                // CASE 2: KHOẢNG CÁCH TRUNG BÌNH -> BẮN
                // (Thêm chút ngẫu nhiên: 70% Bắn, 30% vẫn Đuổi cho áp lực)
                if (Random.value < 0.7f)
                {
                    yield return StartCoroutine(PerformRangedAttack());
                }
                else
                {
                    yield return StartCoroutine(ChasePlayerBehavior());
                }
            }
            else
            {
                // CASE 3: QUÁ XA (Ngoài tầm bắn) -> CHỈ ĐUỔI THEO
                yield return StartCoroutine(ChasePlayerBehavior());
            }

            // Nghỉ nhịp
            if(anim) anim.Play("Idle");
            currentState = BossState.Idle;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // =========================================================
    //               HÀNH VI DI CHUYỂN
    // =========================================================

    IEnumerator ChasePlayerBehavior()
    {
        if(anim) anim.Play("Walk");
        float timer = 0;
        
        // Đuổi cho đến khi vào được tầm MELEE (meleeRange) thì dừng để đánh
        // Hoặc hết 1.5 giây thì dừng để suy nghĩ tiếp
        while (timer < 1.5f && player != null && Vector2.Distance(transform.position, player.position) > meleeRange - 0.5f)
        {
            Vector2 target = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ReturnToStartAndStop()
    {
        currentState = BossState.Resetting;
        isChasing = false; 
        if(anim) anim.Play("Walk");

        while (Vector2.Distance(transform.position, startPosition) > 0.2f)
        {
            if (player != null) yield break; 
            transform.position = Vector2.MoveTowards(transform.position, startPosition, patrolSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPosition;
        currentState = BossState.Idle;
        if(anim) anim.Play("Idle");
        while (player == null) yield return null;
    }

    // =========================================================
    //               CÁC HÀM TẤN CÔNG & HỖ TRỢ
    // =========================================================
    IEnumerator PerformRangedAttack()
    {
        currentState = BossState.Attacking;
        if(anim) anim.Play("Attack_1");
        yield return new WaitForSeconds(0.5f);
        if (player != null && bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Vector2 dir = (player.position - firePoint.position).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
        }
        yield return new WaitForSeconds(0.5f); 
    }

    IEnumerator PerformMeleeAttack()
    {
        currentState = BossState.Attacking;
        if(anim) anim.Play("Attack_3"); 
        yield return new WaitForSeconds(0.5f); 
        Collider2D hit = Physics2D.OverlapCircle(meleePoint.position, meleeRadius);
        if (hit != null && hit.CompareTag("Player"))
        {
            hit.GetComponent<PlayerController>().IsDeath = true;
        }
        yield return new WaitForSeconds(1f);
    }

    public void TryBlock()
    {
        if (currentState != BossState.Dead && currentState != BossState.Defending && Time.time >= nextShieldTime)
        {
            isChasing = true; 
            StopAllCoroutines(); 
            StartCoroutine(ActivateShield());
        }
    }

    IEnumerator ActivateShield()
    {
        currentState = BossState.Defending;
        nextShieldTime = Time.time + shieldCooldown + shieldDuration;
        rb.velocity = Vector2.zero;
        if(anim) anim.Play("Special"); 
        if (shieldVisual) shieldVisual.SetActive(true);
        yield return new WaitForSeconds(shieldDuration);
        if (shieldVisual) shieldVisual.SetActive(false);
        currentState = BossState.Idle;
        StartCoroutine(RunBossLogic());
    }

    public void TakeDamage(float amount)
    {
        if (currentState == BossState.Defending) return;
        currentHealth -= amount;
        isChasing = true; 
        if (currentHealth <= 0) Die();
        else 
        {
             if (currentState == BossState.Patrol || currentState == BossState.Idle)
                if(anim) anim.Play("Hurt");
        }
    }

    void Die()
    {
        currentState = BossState.Dead;
        StopAllCoroutines();
        if(anim) anim.Play("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    void LookAtTarget(Vector3 targetPos)
    {
        if (transform.position.x > targetPos.x) transform.localScale = new Vector3(-1, 1, 1); 
        else transform.localScale = new Vector3(1, 1, 1); 
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        if (Vector2.Distance(transform.position, player.position) > visionRange) return false;
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        if (transform.localScale.x > 0 && dirToPlayer.x > 0) return true; 
        if (transform.localScale.x < 0 && dirToPlayer.x < 0) return true; 
        return false;
    }

    void PatrolBehavior()
    {
        if(anim) anim.Play("Walk");
        if (currentPatrolTarget == null) return; 
        transform.position = Vector2.MoveTowards(transform.position, currentPatrolTarget.position, patrolSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 0.2f)
        {
            if (currentPatrolTarget == pointA) currentPatrolTarget = pointB;
            else currentPatrolTarget = pointA;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 1. Tầm nhìn (Trắng)
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // 2. Tầm Đấm Melee (Đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // 3. Tầm Bắn Shoot (Vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        // 4. Đường đi tuần
        if (pointA && pointB) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
        
        // 5. Vùng sát thương đấm
        if (meleePoint) {
            Gizmos.color = new Color(1, 0, 0, 0.5f); // Đỏ mờ
            Gizmos.DrawWireSphere(meleePoint.position, meleeRadius);
        }
    }
}