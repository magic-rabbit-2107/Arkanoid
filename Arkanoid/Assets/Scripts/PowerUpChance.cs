using UnityEngine;
[System.Serializable]

public enum PowerUpType
{
    ExtraLife,
    LongPaddle,
    FastBall,

    MultiBall
}
public class PowerUpChance
{
    public PowerUpType type;
    [Range(0f, 100f)]
    public float weight;
}