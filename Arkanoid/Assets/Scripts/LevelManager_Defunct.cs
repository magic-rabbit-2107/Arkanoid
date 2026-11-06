using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;



public static class BlockId 
{
    public const int EMPTY = 0;

    public const int WHITE = 1;

    public const int ORANGE = 2;

    public const int CYAN = 3;

    public const int GREEN = 4;

    public const int RED = 5;

    public const int BLUE = 6;

    public const int MAGENTA = 7;

    public const int YELLOW = 8;

    public const int SILVER = 9;
}




public struct CameraDims
{
    public float Width {get; set;}
    public float Height {get; set;}

    public CameraDims(float width, float height)
    {
        Width = width;
        Height = height;
    }
}
public class LevelManager_Defunct : MonoBehaviour
{
    [Header("Parameters")]

    [SerializeField] private GameObject _whiteBlock;

    [SerializeField] private GameObject _orangeBlock;

    [SerializeField] private GameObject _cyanBlock;

    [SerializeField] private GameObject _greenBlock;

    [SerializeField] private GameObject _redBlock;

    [SerializeField] private GameObject _blueBlock;

    [SerializeField] private GameObject _magentaBlock;

    [SerializeField] private GameObject _yellowBlock;

    [SerializeField] private GameObject _silverBlock;


    
    [SerializeField] private float _blockStartYPos;

    [SerializeField] private GameObject _lifeIcon;

    private GameObject _blockLayer;

    private GameObject _lifeLayer;

    private GameObject[] _levelBgObjects;

    private GameObject _levelObject;

    private List<GameObject> _blockObjects;

    private List<GameObject> _lifeIconObjects;

    private int _lifeIndex;

    private static readonly int [,] _blockRows = {
        {9, 9, 9, 9, 9, 9, 9, 9, 9, 9, BlockId.SILVER, BlockId.SILVER, BlockId.SILVER},
        {BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED, BlockId.RED},
        {BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW, BlockId.YELLOW},
        {BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE, BlockId.BLUE},
        {BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA, BlockId.MAGENTA},
        {BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN, BlockId.GREEN}
    };


    private void  Awake() {

        _blockObjects = new List<GameObject>();

        _lifeIconObjects = new List<GameObject>();

        _lifeIndex = Constants.MAX_NUM_LIVES - 1;

       createBlocksLayout();

       createLiveIcons();

       scaleLevelScene();
    }

    private void createBlocksLayout()
    {
         _blockLayer = GameObject.Find("Blocks");

         Vector3 spriteBoundsSize = _greenBlock.GetComponent<SpriteRenderer>().sprite.bounds.size;

         float blockWidth = spriteBoundsSize.x * Constants.SIZE_MULTIPLIER;

         float blockHeight = spriteBoundsSize.y * Constants.SIZE_MULTIPLIER;


         for (int rowIndex = 0; rowIndex < _blockRows.GetLength(0); rowIndex++)
         {
            float width = 0;


            if (rowIndex > 0)
            {
                _blockStartYPos -= blockHeight;
            }


            for (int blockIndex = 0; blockIndex < _blockRows.GetLength(1); blockIndex++)
            {
                int blockId = _blockRows[rowIndex, blockIndex];
                

                if (blockId > 0)
                {
                    GameObject prefab = getBlockPrefab(blockId);
                    GameObject block =  Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
                    block.transform.parent = _blockLayer.transform;

                    float xCoord = Constants.BLOCKS_START_X_POS + width;

                    block.transform.position = new Vector3(xCoord,0,0); 

                    block.transform.position += new Vector3(0,_blockStartYPos,0);

                    block.transform.localScale = new Vector3(1,1,1);
                    _blockObjects.Add(block);
                }
                

                width += blockWidth; 
            }

         }

        
        Debug.Log(_blockObjects.Count);


        // for (int blockIndex = 0; blockIndex < Constants.NUM_BLOCKS_PER_ROW; blockIndex++)
        // {
        //     GameObject block =  Instantiate(_greenBlock, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
        //     block.transform.parent = _blockLayer.transform;

        //     float xCoord = Constants.BLOCKS_START_X_POS + width;

        //     block.transform.position = new Vector3(xCoord,0,0); 
            
        //      width += block.GetComponent<SpriteRenderer>().sprite.bounds.size.x * 0.94f; 


        //     block.transform.position += new Vector3(0,_blockStartYPos,0);
            
        //     block.transform.localScale = new Vector3(1,1,1);
        //     _blockObjects.Add(block);
        // }

    }

    private void scaleLevelScene()
    {
        float camWidth = getCameraDimensions().Width;

        float camHeight = getCameraDimensions().Height;

        bool orientationIsPortrait = camHeight > camWidth ? true : false;

        _levelBgObjects = GameObject.FindGameObjectsWithTag("LevelBgObject");

        float boundsWidth = _levelBgObjects[0].GetComponent<SpriteRenderer>().bounds.size.x;

        float boundsHeight = _levelBgObjects[0].GetComponent<SpriteRenderer>().bounds.size.y;


        _levelObject = GameObject.Find("LevelScene");

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

    private GameObject getBlockPrefab(int blockId)
    {
        GameObject prefab = this._whiteBlock;

        switch (blockId)
        {
            case BlockId.WHITE:
            prefab = this._whiteBlock;
            break;

            case BlockId.ORANGE:
            prefab = this._orangeBlock;
            break;

            case BlockId.CYAN:
            prefab = this._cyanBlock;
            break;

            case BlockId.GREEN:
            prefab = this._greenBlock;
            break;

            case BlockId.RED:
            prefab = this._redBlock;
            break;

            case BlockId.BLUE:
            prefab = this._blueBlock;
            break;

            case BlockId.MAGENTA:
            prefab = this._magentaBlock;
            break;

            case BlockId.YELLOW:
            prefab = this._yellowBlock;
            break;

            case BlockId.SILVER:
            prefab = this._silverBlock;
            break;
        }

        return prefab;
    }

    private void createLiveIcons()
    {
        _lifeLayer = GameObject.Find("Lives");

        float distance = 0;

        for (int lifeIndex = 0; lifeIndex < Constants.MAX_NUM_LIVES; lifeIndex++)
        {
            GameObject life =  Instantiate(_lifeIcon, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            life.transform.parent = _lifeLayer.transform;
            float iconWidth = life.GetComponent<SpriteRenderer>().bounds.size.x;

            float iconX = Constants.LIFE_ICONS_START_X + distance;

            life.transform.position = new Vector3(iconX,0,0); 

            life.transform.position += new Vector3(0,Constants.LIFE_ICONS_START_Y,0);
            distance += iconWidth;

            _lifeIconObjects.Add(life);
        }

        
    }

    public void hideLastLife()
    {
        _lifeIconObjects[_lifeIndex].SetActive(false);
        _lifeIndex -= 1; 
    }

    private CameraDims getCameraDimensions()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        return new CameraDims(width, height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
