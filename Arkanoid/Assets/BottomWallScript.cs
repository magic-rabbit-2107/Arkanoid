using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomWallScript : MonoBehaviour
{

    private bool _ballLost = false;

    private Arena _arena;

    void Start()
    {
        _arena = FindFirstObjectByType<Arena>();
    }

    public bool BallLost
    {
        set { _ballLost = value; }
        get { return _ballLost; }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("collision.gameObject.layer:"+collision.gameObject.layer);

        if (collision.gameObject.layer == 0)
        {
            if (collision.CompareTag("Ball"))
            {
                Debug.Log("Ball object lost");

                _arena.ballSpawner.ballObjects.Remove(collision.gameObject);

                int numBalls = _arena.ballSpawner.getNumberOfBalls();

                if (numBalls <= 0)
                {
                   _ballLost = true;     
                }

                
            } 
            
            Destroy(collision.gameObject);
        }
        
    }
}
