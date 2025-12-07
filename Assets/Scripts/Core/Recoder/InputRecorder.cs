using UnityEngine;
using System.Collections.Generic;

public class InputRecorder : MonoBehaviour
{
    public float sampleInterval = 0.01f;

    List<PlayerInputData> inputs = new List<PlayerInputData>();
    float timer;

    PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (player.IsClone) return;

        timer += Time.deltaTime;
        if (timer >= sampleInterval)
        {
            timer = 0;
            Record();
        }
    }

    void Record()
    {
        PlayerInputData data = new PlayerInputData()
        {
            horizontal = Input.GetAxisRaw("Horizontal"),
            vertical = Input.GetAxisRaw("Vertical"),
            jumpPressed = Input.GetButtonDown("Jump"),
            IsRun =  Input.GetKey(KeyCode.LeftShift),
            dashPressed = Input.GetKeyDown(KeyCode.LeftControl)
        };

        inputs.Add(data);
    }

    public List<PlayerInputData> GetInputs() => inputs;

    public void Clear()
    {
        inputs.Clear();
    }
}