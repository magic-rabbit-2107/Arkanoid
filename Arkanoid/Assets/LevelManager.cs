using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private StateMachine _fsm;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (_fsm == null)
        {
         GenerateFSM();
        }
        _fsm.Update();
    }

    private void GenerateFSM()
        {
            List<BaseState> states = new List<BaseState>() { new GetReadyState(), new PlayingState(), new LoseState(), new WinState(), new GameOverState() };
            _fsm = new StateMachine(gameObject, states);
        }
}
