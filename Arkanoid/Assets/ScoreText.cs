using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        updateScoreValue(0);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void updateScoreValue(int score)
    {
        Transform parent = GameObject.Find("ScoreChars").transform;
    
    float scale = 1f;

    // Clear old characters
    foreach (Transform child in parent)
    {
        Destroy(child.gameObject);
    }

    string scoreText = score.ToString();
    List<Sprite> sprites = new List<Sprite>();
    float totalWidth = 0f;

    // First pass: load sprites and calculate total width
    for (int i = 0; i < scoreText.Length; i++)
    {
        string charToFind = "ScoreChar_" + scoreText[i];
        Sprite sprite = Resources.Load<Sprite>(charToFind);

        if (sprite != null)
        {
            sprites.Add(sprite);
            totalWidth += sprite.bounds.size.x * 1.1f * scale; // account for scale
        }
    }

        float currentX = -totalWidth / 2f;

        

    // Second pass: create characters centered
    for (int i = 0; i < sprites.Count; i++)
    {
        GameObject numberSprite = new GameObject();
        var spriteRenderer = numberSprite.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprites[i];
        spriteRenderer.sortingOrder = 1;

        numberSprite.transform.parent = parent;
        numberSprite.transform.localScale = new Vector3(scale, scale, 1f);

        float charWidth = sprites[i].bounds.size.x * 1.1f * scale;

        numberSprite.transform.localPosition = new Vector3(
            currentX + charWidth / scale,
            0,
            0
        );

        currentX += charWidth;
    }
}
}
