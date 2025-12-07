using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOnlyDown : MonoBehaviour
{
    public GameObject[] items;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            foreach (GameObject item in items)
            {
                Animator anim = item.GetComponentInParent<Animator>();
                anim.SetBool("isActive", false);
                item.SetActive(false);
            }
        }
    }
}
