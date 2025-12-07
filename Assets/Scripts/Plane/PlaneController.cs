using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bombPrefab;
    public GameObject dropPoint;

    public float dropDelay = 1.5f;    
    private bool isActive = false;

    private void Start()
    {
        GetComponent<PlaneController>().ActivatePlane();
    }
    private void Update()
    {
        if (!isActive) return;

        // Máy bay bay sang phải
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    public void ActivatePlane()
    {
        isActive = true;
        Invoke(nameof(DropBomb), dropDelay);
    }

    void DropBomb()
    {
        Instantiate(bombPrefab, dropPoint.transform.position, Quaternion.identity);

        // Nếu có animation thả bom, kích hoạt
        GetComponent<Animator>().SetTrigger("Drop");
    }

    private void OnBecameInvisible()
    {
        // Khi ra khỏi màn hình thì tự xóa
        Destroy(gameObject, 1f);
    }
}
