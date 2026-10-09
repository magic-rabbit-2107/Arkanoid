using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    private PowerUpType _type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public PowerUpType powerUpType
    {
        set { _type = value; }
        get { return _type; }
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if (collisionObject.CompareTag("Paddle"))
        {
            Debug.Log("Power-up collected!");

            PlayerController playerController = collisionObject.GetComponent<PlayerController>();

            playerController.collectPowerUp(_type);

            // Apply your power-up effect here.
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
