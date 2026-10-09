using System;
using UnityEngine;

public class LoseState : BaseState
{

    public override void OnStateEnter(GameObject forGameObject)
    {

        base.OnStateEnter(forGameObject);

                Debug.Log("LoseState-OnStateEnter");


        _arena.setInput(false);

        _arena.showPlayerDeath();

        _arena.playPaddleDestroySound();

        _arena.resetBallLostProperty();

        _arena.GameState.LoseLife();

        _arena.updateLifeIcons();

    }

    public override void OnStateLeave(GameObject forGameObject)
    {
        _arena.destroyPaddle();
        _arena.set1UpLabelState(false);
    }

    public override Type Update(GameObject forGameObject)
    {

       

        if (_arena.hasPlayerDeathAnimationFinshed())
        {
            if (_arena.GameState.lives == 0)
            {
                return typeof(GameOverState);
            }

           return typeof(GetReadyState);
        }

        return GetType();
    }
    
}