using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Trap"))
        {
            playerController.IsDeath = true;
            Debug.Log("XXXX");
        }
    }
}