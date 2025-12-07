using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Settings")]
    public Transform player; // Kéo Player vào đây
    
    [Header("List of Spawn Points")]
    // Danh sách các điểm spawn. Index 0 = Map 1, Index 1 = Map 2, v.v.
    public List<Transform> spawnPoints; 

    // Hàm gọi để đưa player đến map mong muốn
    // mapIndex: Số thứ tự của map trong list (bắt đầu từ 0)
    public void SpawnPlayer(int mapIndex)
    {
        // Kiểm tra xem index có hợp lệ không để tránh lỗi
        if (mapIndex < 0 || mapIndex >= spawnPoints.Count)
        {
            Debug.LogError("Map Index không tồn tại! Kiểm tra lại List Spawn Points.");
            return;
        }

        Transform targetSpawn = spawnPoints[mapIndex];

        // --- Xử lý dịch chuyển Player ---
        
        // CÁCH 1: Nếu Player dùng CharacterController (thường gặp)
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false; // Tắt tạm thời để teleport không bị kẹt
            player.position = targetSpawn.position;
            player.rotation = targetSpawn.rotation;
            cc.enabled = true;
        }
        // CÁCH 2: Nếu Player dùng Rigidbody hoặc chỉ là Transform thường
        else
        {
            player.position = targetSpawn.position;
            player.rotation = targetSpawn.rotation;

            // Reset vận tốc nếu có Rigidbody để tránh player bị trôi
            if (player.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = Vector3.zero;
            }
        }

        Debug.Log($"Đã spawn Player tại Map {mapIndex}: {targetSpawn.name}");
    }
}

