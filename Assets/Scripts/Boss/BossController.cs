using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("--- VISUAL SETUP ---")]
    [Tooltip("Kéo Object cái Khiên vào đây")]
    public GameObject shieldObject;

    [Tooltip("Kéo Object Điểm Yếu (Lõi ngực) vào đây")]
    public GameObject coreWeakPoint;

    [Tooltip("Kéo Empty GameObject (Vị trí tay trái) vào đây")]
    public Transform handGrabPoint;

    [Tooltip("Kéo LineRenderer để làm tia hút vào đây")]
    public LineRenderer tractorBeam;

    [Header("--- SETTINGS ---")]
    public float maxHP = 100f;
    public float moveSpeed = 3f;
    public float eatDuration = 4f;    // Thời gian ăn (Cơ hội để Player đánh)
    public float grabSpeed = 5f;      // Tốc độ hút Clone về tay
    public float shakeIntensity = 0.2f; // Độ rung lắc khi ăn

    private float currentHP;
    private bool isBusy = false;      // Boss đang bận (Ăn hoặc Choáng)
    private Transform currentPrey;    // Clone đang bị tóm
    private Transform playerTransform;

    void Start()
    {
        currentHP = maxHP;

        // Mặc định: Bật khiên, Tắt điểm yếu, Tắt tia hút
        ResetBattleState();

        // Tìm Player để đi theo
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
    }

    void Update()
    {
        if (isBusy || playerTransform == null) return;

        // 1. DI CHUYỂN
        // Chỉ di chuyển theo trục X (Platformer)
        float step = moveSpeed * Time.deltaTime;
        float targetX = playerTransform.position.x;

        // Di chuyển tới vị trí X của Player
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // 2. QUAY MẶT (Flip)
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1); // Quay phải
        else
            transform.localScale = new Vector3(-1, 1, 1); // Quay trái
    }

    // --- CƠ CHẾ TỰ ĐỘNG PHÁT HIỆN CLONE ---
    // Gắn BoxCollider2D (IsTrigger = True) to bao quanh Boss để làm vùng cảm biến
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBusy) return;

        if (other.CompareTag("Clone"))
        {
            StartCoroutine(Routine_GrabAndEat(other.transform));
        }
    }

    // --- LOGIC HÚT & ĂN (Thay thế Animation) ---
    IEnumerator Routine_GrabAndEat(Transform prey)
    {
        isBusy = true;
        currentPrey = prey;

        // Tắt vật lý của Clone để không bị rơi
        Rigidbody2D preyRb = prey.GetComponent<Rigidbody2D>();
        if (preyRb) preyRb.isKinematic = true;

        // GIAI ĐOẠN 1: HÚT VỀ TAY (Tractor Beam)
        tractorBeam.enabled = true;
        float t = 0;
        Vector3 startPos = prey.position;

        while (t < 1f)
        {
            t += Time.deltaTime * grabSpeed;
            // Cập nhật vị trí tia laser
            tractorBeam.SetPosition(0, handGrabPoint.position);
            tractorBeam.SetPosition(1, prey.position);

            // Kéo Clone về tay
            prey.position = Vector3.Lerp(startPos, handGrabPoint.position, t);
            yield return null;
        }

        // GIAI ĐOẠN 2: ĐANG ĂN (Mở khóa điểm yếu)
        prey.SetParent(handGrabPoint); // Gắn chặt vào tay
        prey.localPosition = Vector3.zero;

        shieldObject.SetActive(false);    // TẮT KHIÊN
        coreWeakPoint.SetActive(true);    // MỞ ĐIỂM YẾU (Cho phép đánh)

        Debug.Log("<color=red>BOSS: Lớp giáp đã mở! Đánh ngay!</color>");

        float timer = eatDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // Hiệu ứng Rung lắc (Clone giãy giụa)
            if (currentPrey != null)
            {
                currentPrey.localPosition = Random.insideUnitCircle * shakeIntensity;
                // Cập nhật tia laser rung theo
                tractorBeam.SetPosition(0, handGrabPoint.position);
                tractorBeam.SetPosition(1, currentPrey.position);
            }
            else
            {
                // Nếu Clone bị mất giữa chừng (Code khác xóa)
                break;
            }
            yield return null;
        }

        // GIAI ĐOẠN 3: ĂN XONG (Nếu Player không đánh kịp)
        FinishEating();
    }

    // --- HÀM NHẬN DAMAGE (Player gọi hàm này) ---
    public void TakeDamage(float amount)
    {
        // Chỉ nhận damage khi Core đang mở
        if (!coreWeakPoint.activeSelf) return;

        currentHP -= amount;
        Debug.Log($"BOSS HP: {currentHP}");

        // HIỆU ỨNG BỊ ĐAU -> NHẢ CLONE RA
        if (currentPrey != null)
        {
            // Thả Clone ra
            currentPrey.SetParent(null);
            Rigidbody2D rb = currentPrey.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.AddForce(new Vector2(-transform.localScale.x * 5, 5), ForceMode2D.Impulse); // Bắn văng ra
            }
            currentPrey = null;
        }

        // Reset lại trạng thái chiến đấu ngay lập tức
        StopAllCoroutines();
        ResetBattleState();

        if (currentHP <= 0) Die();
    }

    private void FinishEating()
    {
        if (currentPrey != null) Destroy(currentPrey.gameObject); // Nuốt chửng Clone
        ResetBattleState();
    }

    private void ResetBattleState()
    {
        isBusy = false;
        shieldObject.SetActive(true);     // Bật lại khiên
        coreWeakPoint.SetActive(false);   // Giấu điểm yếu
        tractorBeam.enabled = false;      // Tắt tia
        currentPrey = null;
    }

    private void Die()
    {
        Debug.Log("BOSS DEFEATED!");
        shieldObject.SetActive(false);
        coreWeakPoint.SetActive(false);
        // Thêm logic nổ tung/chiến thắng ở đây
        Destroy(gameObject, 0.5f);
    }
}