
using UnityEngine;

public enum PlayerState { Idle, Walk, Run, Jump, Dash, Die }

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState Current { get; private set; } = PlayerState.Idle;
    public bool IsGrounded { get; set; } = true; 

    public bool CanTransition(PlayerState newState)
    {
        if (Current == PlayerState.Die) 
            return false;

        switch (newState)
        {
            case PlayerState.Jump:
                return IsGrounded && Current != PlayerState.Dash;
                       
            case PlayerState.Dash:
                return Current != PlayerState.Dash;
                
            case PlayerState.Die:
                return true;
                
            case PlayerState.Idle:
            case PlayerState.Walk:
            case PlayerState.Run:
                return IsGrounded && Current != PlayerState.Dash;
                
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
