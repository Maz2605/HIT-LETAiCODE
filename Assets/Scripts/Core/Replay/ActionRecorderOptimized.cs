using UnityEngine;
using System.Collections.Generic;

public class ActionRecorderOptimized : MonoBehaviour
{
    public float sampleInterval = 0.01f;
    public float positionThreshold = 0.001f;

    List<FrameData> frames = new List<FrameData>();
    float timer;

    Rigidbody2D rb;
    Animator anim;
    
    // Để filter state noise
    private int lastRecordedState = -1;
    private int pendingState = -1;
    private int pendingStateCount = 0;
    private const int minFramesBeforeStateChange = 2;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        int currentState = 0;
        int stateToRecord = currentState;
        
        if (lastRecordedState == -1)
        {
            lastRecordedState = currentState;
            pendingState = currentState;
            pendingStateCount = minFramesBeforeStateChange;
        }
        else if (currentState == lastRecordedState)
        {
            stateToRecord = lastRecordedState;
            pendingState = -1;
            pendingStateCount = 0;
        }
        else
        {
            if (currentState != pendingState)
            {
                pendingState = currentState;
                pendingStateCount = 1;
                stateToRecord = lastRecordedState; 
            }
            else
            {
                pendingStateCount++;
                
                if (pendingStateCount >= minFramesBeforeStateChange)
                {
                    lastRecordedState = currentState;
                    stateToRecord = currentState;
                }
                else
                {
                    stateToRecord = lastRecordedState; 
                }
            }
        }
        
        FrameData f = new FrameData()
        {
            pos = transform.position,
            state = stateToRecord,
            facingRight = transform.localScale.x > 0
        };
        frames.Add(f);
    }

    public List<FrameData> GetFrames() => frames;
    
    public void Clear()
    {
        frames = new List<FrameData>();
        lastRecordedState = -1;
        pendingState = -1;
        pendingStateCount = 0;
    }
}

[System.Serializable]
public struct FrameData
{
    public Vector3 pos;
    public int state;
    public bool facingRight;
}