using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
      private GameObject[] _levelBgObjects;

    private GameObject _levelObject;
    // Start is called before the first frame update
    void Start()
    {
      scaleLevelScene();
    }

  
    private void scaleLevelScene() {
        
        float camWidth = getCameraDimensions().Width;

        float camHeight = getCameraDimensions().Height;

        bool orientationIsPortrait = camHeight > camWidth ? true : false;

        _levelBgObjects = GameObject.FindGameObjectsWithTag("LevelBgObject");

        float boundsWidth = _levelBgObjects[0].GetComponent<SpriteRenderer>().bounds.size.x;

        float boundsHeight = _levelBgObjects[0].GetComponent<SpriteRenderer>().bounds.size.y;


        _levelObject = GameObject.Find("Arena");

        if (orientationIsPortrait)
        {
            float widthMultiplier = camWidth / boundsWidth;

            float heightMultiplier = widthMultiplier * 1.07f;
            _levelObject.transform.localScale = new Vector3(widthMultiplier, heightMultiplier, 1);

            
        }
        else
        {
            float heightMultiplier = camHeight / boundsHeight;

            float widthMultiplier = heightMultiplier / 1.07f;

            _levelObject.transform.localScale = new Vector3(widthMultiplier, heightMultiplier, 1);

        }

    }

    private CameraDims getCameraDimensions()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        return new CameraDims(width, height);
    }
}
