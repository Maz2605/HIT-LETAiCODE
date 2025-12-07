using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public List<ButtonPlane> buttons = new List<ButtonPlane>();
    

    private void Start()
    {
        foreach (var btn in buttons)
            btn.Init(this);
    }

    public void NotifyButtonStateChanged()
    {
        foreach (var btn in buttons)
        {
            if (!btn.isPressed)
            {
                // có 1 nút chưa kích hoạt → toàn bộ chưa sẵn sàng
                //OnAnyButtonReleased?.Invoke();
                return;
            }
        }

        // tất cả nút đều được đè lên
        //OnAllButtonsActivated?.Invoke();
        Debug.Log("plane");
    }
}
