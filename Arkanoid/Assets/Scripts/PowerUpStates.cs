using UnityEngine;

[CreateAssetMenu(menuName = "Game/Power Up State")]
public class PowerUpState : ScriptableObject
{
    public bool catchActive;


    public void ResetPowerUps()
    {
        catchActive = false;
    }
}