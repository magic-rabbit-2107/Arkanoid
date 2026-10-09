using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public abstract class Arena : MonoBehaviour
{
    [Header("Game Objects")]

    // [SerializeField] private GameObject _whiteBlock;

    // [SerializeField] private GameObject _orangeBlock;

    // [SerializeField] private GameObject _cyanBlock;

    // [SerializeField] private GameObject _greenBlock;

    // [SerializeField] private GameObject _redBlock;

    // [SerializeField] private GameObject _blueBlock;

    // [SerializeField] private GameObject _magentaBlock;

    // [SerializeField] private GameObject _yellowBlock;

    // [SerializeField] private GameObject _silverBlock;

    [SerializeField] private float _blockStartYPos;

    // [SerializeField] private GameObject _lifeIcon;

    // [SerializeField] private GameObject _paddle;

    // [SerializeField] private GameObject _ballPrefab;



    [Header("Paddle parameters")]
    [SerializeField] private float _paddleSpeed = 4f;

    [Header("Game State")]
    [SerializeField] 
    private GameState _gameState;

    public GameState GameState => _gameState;


    private GameObject[] _levelBgObjects;

    private GameObject _levelObject;

    private GameObject _blockLayer;

    private List<GameObject> _blockObjects;

    private GameObject _paddleObject;

    private GameObject _ballObject;

    public List<GameObject> ballObjects {get; set; }

    protected abstract int [,] BlockTypeMap  {get;}


    public int Rows() => BlockTypeMap.GetLength(0);
    public int Columns() => BlockTypeMap.GetLength(1);

    private AudioSource _audioSource; 

    private Vector2 _movement;

    private float _playAreaMinXPos = -3.2f;

    private float _playAreaMaximumXPos = 3.2f;

    private bool _isBallInPlay = false;

    private bool _fireButtonPressed = false;

    private GameObject _scoreText;


    private PlayerInput _playerInput;

    private Vector2 moveVector = Vector2.zero;

    private BallSpawner _ballSpawner;

    private BottomWallScript _bottomWallScript;

    private int _blocksRemaining;

    private int _scoreValue;

    private GameObject _1UPLabel;

[SerializeField]
    private GameObject _gameOverObject;

[SerializeField] private LevelDatabase _database;

 [Header("Power Up")]
    [SerializeField] 
    private PowerUpState _powerUpState;

    public PowerUpState PowerUpState => _powerUpState;

    protected virtual void Awake()
    {
        _blockObjects = new List<GameObject>();
        _audioSource = GetComponent<AudioSource>();
        _playerInput = new PlayerInput();
        _ballSpawner = gameObject.AddComponent<BallSpawner>();
        _scoreText = GameObject.Find("ScoreChars");
        _1UPLabel = GameObject.Find("1UP LABEL");

    }

    private void  OnEnable() {
    }

    // Start is called before the first frame update
    void Start()
    {
        createBlocksLayout();
        scaleLevelScene();

        _playerInput.Enable();
        _playerInput.Gameplay.Move.performed += onMovementPerformed;
        _playerInput.Gameplay.Move.performed += OnMovementCancelled;
        _ballSpawner.SetBallPrefab(_database.ballPrefab);

        _scoreValue = 0;
    
    }

    private void onMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }

    private void createBlocksLayout()
    {
        _blockLayer = GameObject.Find("Blocks");

        Vector3 spriteBoundsSize = getBlockPrefab(BlockType.white).GetComponent<SpriteRenderer>().sprite.bounds.size;

        float blockWidth = spriteBoundsSize.x * Constants.SIZE_MULTIPLIER;

         float blockHeight = spriteBoundsSize.y * Constants.SIZE_MULTIPLIER;

        for (int rowIndex = 0; rowIndex < Rows(); rowIndex++)
        {
            float width = 0;

            if (rowIndex > 0)
            {
                _blockStartYPos -= blockHeight;
            }

            for (int colIndex = 0; colIndex < Columns(); colIndex++)
            {
                BlockType blockType = GetBlockType(rowIndex, colIndex);

                if (blockType != BlockType.empty)
                {
                    GameObject prefab = getBlockPrefab(blockType);
                    GameObject block = Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
                    block.transform.parent = _blockLayer.transform;

                    float xCoord = Constants.BLOCKS_START_X_POS + width;

                    block.transform.position = new Vector3(xCoord, 0, 0);

                    block.transform.position += new Vector3(0, _blockStartYPos, 0);

                    block.transform.localScale = new Vector3(1, 1, 1);
                    _blockObjects.Add(block);
                }

                width += blockWidth;

            }
        }

        _blocksRemaining = _blockObjects.Count;

    }

    private BlockType GetBlockType(int row, int col)
    {
        return (BlockType)BlockTypeMap[row, col];
    }

    private GameObject getBlockPrefab(BlockType blockType)
    {
        return _database.blockPrefabs.Find(x => x.type == blockType).prefab;
    }
    

    private void scaleLevelScene() {
        
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

    private CameraDims getCameraDimensions()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        return new CameraDims(width, height);
    }

    public void playGetReadySound()
    {
        _audioSource.PlayOneShot(this._database.getReadySound);
    }

    public void showGetReadyMessage()
    {
        GameObject roundMesage = GameObject.Find("GetReadyMessage");
        
        GameObject roundNumber = roundMesage.transform.Find("RoundNumber").gameObject;

        GameObject getReadyMsg = roundMesage.transform.Find("GetReady").gameObject;

        StartCoroutine(DelaySetActive(0.5f,roundNumber,true));

        StartCoroutine(DelaySetActive(1f,getReadyMsg,true));

        StartCoroutine(DelaySetActive(2f,roundNumber,false));

        StartCoroutine(DelaySetActive(2f,getReadyMsg,false));

    }

    public void showGameOverMessage()
    {
                Debug.Log(_gameOverObject);


        StartCoroutine(DelaySetActive(0.5f,_gameOverObject,true));

    }

    IEnumerator DelaySetActive(float delayTime, GameObject objectToShow, bool active)
    {
        yield return new WaitForSeconds(delayTime);

        objectToShow.SetActive(active);
    }

    IEnumerator DelayShowPaddle(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        _paddleObject.SetActive(true);

        _paddleObject.GetComponent<PlayerController>().showPlayerStart();
    }

    public void updateLifeIcons()
    {
        int numberLives = _gameState.lives;


        GameObject livesGameObject = GameObject.Find("Lives");

        // Remove existing life icons
        foreach (Transform child in livesGameObject.transform)
        {
            Destroy(child.gameObject);
        }

        livesGameObject.transform.position = new Vector3(0,0,0);

        float width = 0;

        for (int i = 0; i < numberLives; i++)
        {
           GameObject lifeIcon =  Instantiate(this._database.lifeIcon, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
           lifeIcon.transform.parent = livesGameObject.transform;
           lifeIcon.transform.localScale = new Vector3(1.2f,1.1f,1);

           float xCoord = Constants.LIFE_ICONS_START_X + width;

           lifeIcon.transform.position = new Vector3(xCoord,Constants.LIFE_ICONS_START_Y,0);

            width += lifeIcon.transform.localScale.x * 0.55f;


        }


    }

    public void addPaddle()
    {
            _paddleObject = Instantiate(
            this._database.paddle,
            new Vector3(0f, Constants.PADDLE_Y, 0f),
            Quaternion.identity
        );
            _paddleObject.transform.parent = _levelObject.transform;
            _paddleObject.transform.localScale = new Vector3(1f,1f,1);
        _paddleObject.SetActive(false);
            

    }



    public void showPaddle(float delayTime)
    {
        StartCoroutine(DelayShowPaddle(delayTime));
    }

    public void addBall()
    {
        Vector3 paddlePosition = _paddleObject.transform.position;


        _ballObject = _ballSpawner.createBall();

        Ball ballScript = _ballObject.GetComponent<Ball>();

        ballScript.AttachToPaddle(_paddleObject.transform);

        _paddleObject.GetComponent<PlayerController>().SetBall(ballScript);
       
       // _ballObject.transform.parent = _levelObject.transform;
        _ballObject.transform.localScale = new Vector3(1f,1f,1);
        _ballObject.SetActive(true);

        
        this.ballObjects = new List<GameObject>
        {
            _ballObject
        };

    }

    public void showBall(float delayTime)
    {
        StartCoroutine(DelaySetActive(delayTime, _ballObject, true));
    }

    public void setInput(bool input)
    {
        _paddleObject.GetComponent<PlayerController>().setInput(input);
    }
 

    public void Move(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
        Debug.Log(_movement);
    }

    public void Fire(InputAction.CallbackContext context) {
        _fireButtonPressed = context.action.triggered && context.action.ReadValue<float>() > 0 & context.action.phase == InputActionPhase.Performed;
    }

   private void FixedUpdate()
   {
   }

    // Update is called once per frame
    void Update()
    {

    }
    
    public bool hasPlayerStartAnimationFinished()
    {
        return _paddleObject.GetComponent<PlayerController>().AnimationStateCompleted("Player_Start");
    }

    public bool isBallInPlay()
    {
        BottomWallScript bottomWallScript = GameObject.Find("BottomWall").GetComponent<BottomWallScript>();

        return !bottomWallScript.BallLost;
    }

    public void showPlayingState()
    {
        _paddleObject.GetComponent<PlayerController>().showPlayingState();

        set1UpLabelState(true);

    }

    public void showPlayerDeath()
    {
        PlayerController playerController = _paddleObject.GetComponent<PlayerController>();

        playerController.showPlayerDeath();

    }

    public bool hasPlayerDeathAnimationFinshed()
    {
        return _paddleObject.GetComponent<PlayerController>().AnimationStateCompleted("Player_Destroy");
    }

    public void playPaddleDestroySound()
    {
        _audioSource.PlayOneShot(this._database.destroyPaddleSound);
    }

    public void destroyPaddle()
    {
        Destroy(_paddleObject);
    }

    public void destroyBall()
    {
        Destroy(_ballObject);
    }

    public void resetBallLostProperty()
    {
        BottomWallScript bottomWallScript = GameObject.Find("BottomWall").GetComponent<BottomWallScript>();
        bottomWallScript.BallLost = false;
    }

    public void set1UpLabelState(bool blinking)
    {
        SpriteBlink spriteBlink = _1UPLabel.GetComponent<SpriteBlink>();
        spriteBlink.SetPlayingState(blinking);


    }

    public void playExtraLifeSound()
    {
        _audioSource.PlayOneShot(this._database.extraLifeSound);
    }

    public bool gameOverMessageIsShown()
    {
        return _gameOverObject.activeSelf;
    }

    public int BlocksRemaining
    {
        set { _blocksRemaining = value; }
        get { return _blocksRemaining; }
    }

    public int ScoreValue
    {
        set {
            _scoreValue = value;
            ScoreText scoreTextScript = _scoreText.GetComponent<ScoreText>();
            scoreTextScript.updateScoreValue(_scoreValue);
            _gameState.AddScore(value);
        }
        get { return _scoreValue; }
    }


}
