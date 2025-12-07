using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroller : MonoBehaviour
{
    public float scrollSpeed = 100f;
    public Transform startPos;
    public float stopY = 1200f;

    private bool finished = false;

    void Start()
    {
        transform.localPosition = startPos.localPosition;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (finished)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("Menu");
            }
            return;
        }


        float speed = Input.GetMouseButton(0) ? 200f : scrollSpeed;


        transform.localPosition += new Vector3(0, speed * Time.deltaTime, 0);


        if (transform.localPosition.y >= stopY)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, stopY, transform.localPosition.z);
            finished = true;
        }
    }
}
