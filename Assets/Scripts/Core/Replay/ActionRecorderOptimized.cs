
using UnityEngine;
using System.Collections.Generic;

public class ActionRecorderOptimized : MonoBehaviour
{
    public float sampleInterval = 0.05f;
    public float positionThreshold = 0.001f;

    List<FrameData> frames = new List<FrameData>();
    float timer;

    Rigidbody2D rb;
    /*
    Animator anim;
    */

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        /*
        anim = GetComponentInChildren<Animator>();
    */
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= sampleInterval)
        {
            timer = 0;
            Record();
        }
    }

    void Record()
    {
        FrameData f = new FrameData()
        {
            pos = transform.position,
            vel = rb.velocity,
            /*
            animState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash
        */
        };
        frames.Add(f);
    }

    public List<FrameData> GetFrames() => frames;
    public void Clear() => frames = new List<FrameData>();
}

[System.Serializable]
public struct FrameData
{
    public Vector3 pos;
    public Vector2 vel;
    public int animState;
}
