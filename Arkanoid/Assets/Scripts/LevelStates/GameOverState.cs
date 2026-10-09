using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverState : BaseState
{
    public override void OnStateEnter(GameObject forGameObject)
    {
        base.OnStateEnter(forGameObject);

        Debug.Log("GameOverState-OnStateEnter");

        _arena.showGameOverMessage();

        
    }

    public override Type Update(GameObject forGameObject)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
                    Debug.Log("1. Space pressed");
        Debug.Log("2. About to load TitleScreen");

        SceneManager.LoadScene("TitleScreen");

        Debug.Log("3. LoadScene returned");

            // SceneManager.LoadScene("TitleScreen");
        }

        return GetType();
    }
}