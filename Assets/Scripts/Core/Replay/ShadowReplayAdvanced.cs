using UnityEngine;
using System.Collections.Generic;

public class ShadowReplayAdvanced : MonoBehaviour
{
    List<FrameData> frames;
    int index = 0;
    float timer = 0f;
    private float playbackInterval = 0.05f; 

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
        rb.simulated = false;
    }

    public void LoadFrames(List<FrameData> f)
    {
        frames = new List<FrameData>(f);
        index = 0;
        timer = 0f;
    }

    void Update()
    {
        if (frames == null || frames.Count == 0) return;

        timer += Time.deltaTime;
        
        if (timer >= playbackInterval)
        {
            timer -= playbackInterval;
            index++;

            if (index >= frames.Count)
                index = 0;
        }
        if (index < frames.Count - 1)
        {
            float t = timer / playbackInterval;
            Vector3 currentPos = frames[index].pos;
            Vector3 nextPos = frames[index + 1].pos;
            
            transform.position = Vector3.Lerp(currentPos, nextPos, t);
            
            /*
            if (t < 0.5f)
                anim.Play(frames[index].animState, 0);
            */
        }
        else
        {
            transform.position = frames[index].pos;
        }
    }
}