using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;

public class BlockScript : MonoBehaviour
{

    [Header("Block Parameters")]
    [SerializeField] private int _numberOfHits = 1;
    [SerializeField] private int _pointValue = 1;

    [SerializeField] private GameObject _capsule;

    private List<PowerUpChance> _powerUps = new List<PowerUpChance>
    {
        new PowerUpChance { type = PowerUpType.ExtraLife, weight = 10f },
        new PowerUpChance { type = PowerUpType.LongPaddle, weight = 30f },
        new PowerUpChance { type = PowerUpType.FastBall, weight = 20f },
        new PowerUpChance { type = PowerUpType.MultiBall, weight = 20f }
    };

    private PowerUpType? _powerUpType;

    /// <summary>[Editor-Only]
    /// Records the number of times that the block has been hit by the ball
    /// </summary>
    private int _numHits = 0;

    private Arena _arenaScript;
    
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private Material _material;

    [SerializeField] private float _flashDuration = 0.2f;

    [SerializeField] private float _powerUpChance = 0.45f;



    // Start is called before the first frame update
    void Start()
    {
        tag = "Block";

        _arenaScript = GameObject.Find("LevelScene").GetComponent<Arena>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;

        _material = _spriteRenderer.material;
       
        AddRandomPowerUp();
        


    }

    private void AddRandomPowerUp()
    {
        float roll = Random.value;

        Debug.Log(
            $"AddPowerUp called | Roll={roll} | Chance={_powerUpChance}"
        );
        
        if (roll > _powerUpChance) return;

        Debug.Log("PowerUpAdded");

        SelectPowerUp();

    }

   private void SelectPowerUp()
    {
        float totalWeight = 0f;

        foreach (PowerUpChance powerUp in _powerUps)
            totalWeight += powerUp.weight;

        float roll = Random.value * totalWeight;

        foreach (PowerUpChance powerUp in _powerUps)
        {
            if (roll < powerUp.weight)
            {
                _powerUpType = powerUp.type;
                return;
            }

            roll -= powerUp.weight;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _numHits += 1;

        if (_numHits < _numberOfHits)
        {
            StartCoroutine(FlashWhite());
        }

        if (_numHits == _numberOfHits)
        {

            _arenaScript.BlocksRemaining -= 1;

            _arenaScript.ScoreValue += _pointValue;


            Destroy(gameObject);

            
            ReleasePowerUp();

        }


    }

    private void ReleasePowerUp()
    {
        if (_powerUpType == null) return;

        Debug.Log($"Release Power Up called: {_powerUpType}");

        Vector3 localScale = GameObject.Find("LevelScene").transform.localScale;

        GameObject capsule = Instantiate(_capsule, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);

        PowerUp powerUp = capsule.GetComponent<PowerUp>();

        powerUp.powerUpType = _powerUpType.Value;

        capsule.transform.parent = GameObject.Find("Blocks").transform; 

        capsule.transform.localScale = new Vector3(0.89f, 0.92f, 1);
            

    }
    
    private IEnumerator FlashWhite()
    {

        _material.SetColor("_Color", new Color(3f, 3f, 3f, 1f));

        yield return new WaitForSeconds(_flashDuration);

        _material.SetColor("_Color", Color.white);
    
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
