using System;
using UnityEngine;

public class PlayingState : BaseState
{
    public override void OnStateEnter(GameObject forGameObject)
    {
        Debug.Log("PlayingState-OnStateEnter");

        base.OnStateEnter(forGameObject);

        _arena.setInput(true);

        _arena.addBall();

        _arena.showPlayingState();
    }

    public override void OnStateLeave(GameObject forGameObject)
    {
        Debug.Log("PlayingState-OnStateLeave");

        _arena.setInput(false);
    }

    public override Type Update(GameObject forGameObject)
    {

        if (_arena.BlocksRemaining == 0)
        {
            return typeof(WinState);
        }

        if (!_arena.isBallInPlay())
        {
            return typeof(LoseState);
        }


    
            return GetType();
    }
}