using UnityEngine;
using System.Collections.Generic;

public class ShadowReplayInput : MonoBehaviour
{
    List<PlayerInputData> inputs;
    int index = 0;
    float timer = 0f;

    public float interval = 0.01f;

    PlayerController controller;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        controller.IsClone = true;  
    }

    public void LoadInputs(List<PlayerInputData> list)
    {
        inputs = new List<PlayerInputData>(list);
        index = 0;
    }

    void Update()
    {
        if (inputs == null || inputs.Count == 0) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer -= interval;

            if (index < inputs.Count)
            {
                controller.cloneInput = inputs[index];
                index++;
            }
            else
            {
                enabled = false;

                controller.cloneInput = new PlayerInputData();
            }
        }
    }
}