using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundNumber : MonoBehaviour
{
    [Header("Round Parameters")]
    [SerializeField] private int _roundNumber;

    void Awake()
    {

        SetupRoundUI(_roundNumber);
    }
    
    private void SetupRoundUI(int roundNumber)
    {
        string scoreText = roundNumber.ToString();
        float roundPrefixWidth = 0;

            GameObject roundNumberMessage = GameObject.Find("RoundNumber");
            roundNumberMessage.SetActive(false);
            roundNumberMessage.transform.position -= new Vector3(0, 2f, 0);

            // ----- Create Prefix -----
            GameObject prefixObject = new GameObject("RoundPrefix");
            var prefixSpriteRender = prefixObject.AddComponent<SpriteRenderer>();
            prefixSpriteRender.sprite = Resources.Load<Sprite>("Round_prefix");
            prefixSpriteRender.sortingOrder = 1;

            roundPrefixWidth += prefixSpriteRender.bounds.size.x * 0.25f;
            prefixObject.transform.position -= 
                new Vector3(prefixSpriteRender.bounds.size.x * 0.5f, 0, 0);

            prefixObject.transform.parent = roundNumberMessage.transform;

            // ----- Create Number Sprites -----
            for (int charIndex = 0; charIndex < scoreText.Length; charIndex++)
            {
                string charToFind = "ScoreChar_" + scoreText[charIndex];

                GameObject numberSprite = new GameObject();
                var spriteRenderer = numberSprite.AddComponent<SpriteRenderer>();

                spriteRenderer.sprite = Resources.Load<Sprite>(charToFind);
                spriteRenderer.sortingOrder = 1;
                numberSprite.name = spriteRenderer.sprite.name;

                roundPrefixWidth += spriteRenderer.bounds.size.x;

                numberSprite.transform.position += 
                    new Vector3(roundPrefixWidth, 0, 0);

                numberSprite.transform.parent = roundNumberMessage.transform;
            }

            // Center the whole message
            roundNumberMessage.transform.position += 
                new Vector3(roundPrefixWidth * 0.5f, 0, 0);

            // ----- Setup "Get Ready" Message -----
            GameObject readyMessage = GameObject.Find("GetReady");
            readyMessage.SetActive(false);

            var readyMsgSpriteRenderer = readyMessage.AddComponent<SpriteRenderer>();
            readyMsgSpriteRenderer.sprite = Resources.Load<Sprite>("Ready_message");
            readyMsgSpriteRenderer.sortingOrder = 1;

            readyMessage.transform.position -= 
                new Vector3(0, prefixSpriteRender.bounds.size.y * 2.5f, 0);

            readyMessage.transform.parent = transform;

            // Final vertical adjustment
            transform.position -= new Vector3(0, 0.5f, 0);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
