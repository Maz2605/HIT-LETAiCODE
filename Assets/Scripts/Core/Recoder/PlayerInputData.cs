[System.Serializable]
public struct PlayerInputData
{
    public float horizontal;
    public float vertical;
    public bool runHeld;
    public bool jumpPressed;
    public bool dashPressed;

    public PlayerInputData(float h, float v, bool run, bool jump, bool dash)
    {
        horizontal = h;
        vertical = v;
        runHeld = run;
        jumpPressed = jump;
        dashPressed = dash;
    }
}
