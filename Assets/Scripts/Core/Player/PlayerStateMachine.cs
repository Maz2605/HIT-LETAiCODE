using UnityEngine;

public enum PlayerState 
{ 
    Idle = 0,
    Walk = 1,
    Run = 2,
    Jump = 3,
    Fall = 4,
    Dash = 5,
    Hurt = 6,
    Die = 7,
    WallSlide = 8,
    WallJump = 9,
    LedgeHang = 10,
    LedgeClimb = 11,
    Slide = 12,
    Turn = 13,
    RunToIdle = 14
}

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState Current { get; private set; } = PlayerState.Idle;
    public bool IsGrounded { get; set; } = true;
    public bool IsOnWall { get; set; } = false;
    public bool IsOnLedge { get; set; } = false;

    public bool CanTransition(PlayerState newState)
    {
        if (Current == PlayerState.Die) 
            return false;

        if (Current == PlayerState.Hurt && newState != PlayerState.Die && newState != PlayerState.Idle)
            return false;

        switch (newState)
        {
            case PlayerState.Jump:
                return IsGrounded && Current != PlayerState.Dash && Current != PlayerState.Hurt;

            case PlayerState.WallJump:
                return IsOnWall && !IsGrounded;

            case PlayerState.Dash:
                return Current != PlayerState.Dash && Current != PlayerState.Hurt;

            case PlayerState.Fall:
                return !IsGrounded;

            case PlayerState.Slide:
                return IsGrounded &&
                       (Current == PlayerState.Run || Current == PlayerState.Walk);

            case PlayerState.Turn:
                return IsGrounded &&
                       (Current == PlayerState.Run || Current == PlayerState.Walk);

            case PlayerState.RunToIdle:
                return IsGrounded &&
                       (Current == PlayerState.Run || Current == PlayerState.Walk);

            case PlayerState.Idle:
            case PlayerState.Walk:
            case PlayerState.Run:
                return IsGrounded &&
                       Current != PlayerState.Dash &&
                       Current != PlayerState.Hurt &&
                       Current != PlayerState.Turn &&
                       Current != PlayerState.RunToIdle;

            default:
                return true;
        }
    }

    public bool TryChange(PlayerState newState)
    {
        if (!CanTransition(newState))
            return false;

        Current = newState;
        return true;
    }

    public void Change(PlayerState s)
    {
        Current = s;
    }
}
