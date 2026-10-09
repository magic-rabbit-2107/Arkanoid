using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomWallScript : MonoBehaviour
{

    private bool _ballLost = false;

    // private LevelManager_Defunct _levelScript;

    void Start()
    {

        // GameObject obj = GameObject.Find("LevelScene");

        // if (obj == null)
        // {
        //     Debug.LogError("LevelScene NOT found");
        //     return;
        // }

        // _levelScript = GameObject.Find("LevelScene").GetComponent<LevelManager_Defunct>();

        // if (_levelScript == null)
        // {
        // Debug.LogError("LevelManager_Defunct NOT found on LevelScene");
        // }

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
            collision.gameObject.
            _ballLost = true;
            Destroy(collision.gameObject);
        }
        
    }
}
