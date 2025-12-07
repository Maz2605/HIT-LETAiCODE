using UnityEngine;

public class BossSensor : MonoBehaviour
{
    private BossController boss;

    void Start()
    {
        boss = GetComponentInParent<BossController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Yêu cầu: Đạn của Player phải có Tag là "PlayerBullet"
        if (other.CompareTag("Player"))
        {
            if (boss != null)
            {
                boss.TryBlock();
            }
        }
    }
}