
using UnityEngine;
using System.Collections.Generic;

public class ShadowReplayAdvanced : MonoBehaviour
{
    List<FrameData> frames;
    int currentIndex = 0;
    float timer = 0f;
    private float playbackInterval = 0.01f;
    
    /*
    Rigidbody2D rb;
    */
    Animator anim;
    int lastState = -1;

    void Awake()
    {
        /*
        rb = GetComponent<Rigidbody2D>();
        */
        anim = GetComponent<Animator>();
    }

    public void LoadFrames(List<FrameData> f)
    {
        frames = new List<FrameData>(f);
        currentIndex = 0;
        timer = 0f;
        lastState = 0;
        
        if (frames.Count > 0)
        {
            anim.SetInteger("State", frames[0].state);
            lastState = frames[0].state;
        }
    }

    void Update()
    {
        if (frames == null || frames.Count == 0) return;

        timer += Time.deltaTime;
        
        float t = Mathf.Clamp01(timer / playbackInterval);
        
        if (timer >= playbackInterval)
        {
            timer -= playbackInterval;
            currentIndex++;
            
            if (currentIndex >= frames.Count)
            {
                currentIndex = 0;
                /*
                lastState = -1;
            */
            }
        }
        
        if (currentIndex < frames.Count - 1)
        {
            Vector3 currentPos = frames[currentIndex].pos;
            Vector3 nextPos = frames[currentIndex + 1].pos;
            
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(currentPos, nextPos, smoothT);
        }
        else
        {
            transform.position = frames[currentIndex].pos;
        }
        
        int currentState = frames[currentIndex].state;
        if (currentState != lastState)
        {
            anim.SetInteger("State", currentState);
            lastState = currentState;
        }
        
        bool facingRight = frames[currentIndex].facingRight;
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}