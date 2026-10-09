using System;
using UnityEngine;


public class GetReadyState : BaseState
{

    private int _frame = 0;

    private const int secondsInReadyState = 4;

    private const int numFramesPerSecond = 60;

    public override void OnStateEnter(GameObject forGameObject)
    {

        base.OnStateEnter(forGameObject);

                Debug.Log("GetReadyState-OnStateEnter");


        //Update the number of ships 
        _arena.updateLifeIcons();
        //Play get ready music

        _arena.playGetReadySound();
        //Show the round number text and after a slight dealy show the get ready message
        _arena.showGetReadyMessage();

        //Create a new paddle and show animation  
        _arena.addPaddle();

        _arena.setInput(false);

        _arena.showPaddle(3f);


        //Add ball in it's initial position
        // _arenaScript.addBall();

        // _arenaScript.showBall(3.5f);

    }
    

   public override Type Update(GameObject forGameObject)
    {


        if (_arena.hasPlayerStartAnimationFinished())
        {
            return typeof(PlayingState);
        }
            // _frame++;

            // if (_frame >= numFramesPerSecond * secondsInReadyState)
            // {
            //     return typeof(PlayingState);
            // }
            
            return GetType();
    }
}
