using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update

    private bool _spacePressed = false;

    [Header("Game State")]
    [SerializeField] 
    private GameState _gameState;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onSpacePressed();
    }
    
     private void onSpacePressed()
    {
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);

        if (spacePressed)
        {
              _gameState.StartNewGame();
              SceneManager.LoadScene("Level1"); 
        }        
    }
}
