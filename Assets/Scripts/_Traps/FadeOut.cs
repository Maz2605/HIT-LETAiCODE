using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeOut : MonoBehaviour
{
    public Tilemap _tile;
    public float fadeSp = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        Color c = _tile.color;
        while(c.a > 0f)
        {
            c.a -= fadeSp * Time.deltaTime;
            _tile.color = c;
            yield return null;
        }
    }    
}
