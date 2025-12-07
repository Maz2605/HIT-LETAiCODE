using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlane : MonoBehaviour
{
    public Sprite normalSprite;      // sprite bình thường
    public Sprite pressedSprite;     // sprite khi bị ấn
    public float pressDepth = 0.05f; // độ lún
    public float pressSpeed = 10f;   // tốc độ lún

    public bool isPressed = false;
    private Vector3 originalPos;
    private SpriteRenderer sr;

    private ButtonManager manager;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPos = transform.localPosition;
    }

    public void Init(ButtonManager mgr) { manager = mgr; }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        isPressed = true;
        sr.sprite = pressedSprite;
        StopAllCoroutines();
        StartCoroutine(PressDown());
        isPressed = true;
        Debug.Log($"{name}: Player đứng lên → Kích hoạt");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        isPressed = false;
        sr.sprite = normalSprite;
        StopAllCoroutines();
        StartCoroutine(PressUp());
        isPressed= false;
        Debug.Log($"{name}: Player rời khỏi → Tắt");
    }

    IEnumerator PressDown()
    {
        Vector3 target = originalPos - new Vector3(0, pressDepth, 0);

        while (Vector3.Distance(transform.localPosition, target) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * pressSpeed);
            yield return null;
        }
    }

    IEnumerator PressUp()
    {
        while (Vector3.Distance(transform.localPosition, originalPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * pressSpeed);
            yield return null;
        }
    }
}
