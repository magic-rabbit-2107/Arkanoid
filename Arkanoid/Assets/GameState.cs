using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Game/Game State")]
public class GameState : ScriptableObject
{
    [Header("Game")]
    public int score;
    public int lives;
    public int level;
    public bool gameStarted;

    public void StartNewGame()
    {
        score = 0;
        lives = 3;
        level = 1;
        gameStarted = true;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void LoseLife()
    {
        lives--;
    }

    public void AddLife()
    {
        lives++;
    }
}
