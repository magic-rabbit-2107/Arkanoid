using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomWallScript : MonoBehaviour
{

    private bool _ballLost = false;

    void Start()
    {

    }

    public bool BallLost
    {
        set { _ballLost = value; }
        get { return _ballLost; }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 0)
        {
            if (collision.CompareTag("Ball"))
            {
                _ballLost = true;
            }
            
            Destroy(collision.gameObject);
        }
        
    }
}
