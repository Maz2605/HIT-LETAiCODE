using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    public GameObject[] items;

    void Start()
    {
        SetState(isPressed: false);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetState(isPressed: true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetState(isPressed: false);
        }
    }

    void SetState(bool isPressed)
    {
        for (int i = 0; i < items.Length; i++)
        {
            Animator anim = items[i].GetComponentInParent<Animator>();
            if (isPressed)
            {
                anim.SetBool("isActive", i % 2 != 0);
                items[i].SetActive(i % 2 != 0);
            }
            else
            {
                anim.SetBool("isActive", i % 2 == 0);
                items[i].SetActive(i % 2 == 0);
            }
        }
    }
}
