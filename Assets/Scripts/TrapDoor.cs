using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Debug.Log("Vào cửa tử -> Về Level 1");
            // Load thẳng index 0 (Màn 1)
            // Không hiện bảng Win, chỉ chuyển cảnh luôn
            GameManager.Instance.LoadLevel(0);
        }
    }
}
