using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ButtonTrigger : MonoBehaviour
{
    public GameObject[] items;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (GameObject item in items)
            {
                Animator anim = item.GetComponentInParent<Animator>();
                if (anim != null)
                    anim.SetBool("isActive", true);
                item.SetActive(true);
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        foreach (GameObject item in items)
        {
            Animator anim = item.GetComponentInParent<Animator>();
            if (anim != null)
                anim.SetBool("isActive", false);
            item.SetActive(false);
        }
    }
}
