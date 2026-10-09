using UnityEngine;
using System.Collections;

public class SpriteBlink : MonoBehaviour
{
    public float blinkInterval = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Coroutine blinkCoroutine;
    private bool isPlaying = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPlayingState(bool playing)
    {
        if (playing && !isPlaying)
        {
            isPlaying = true;
            blinkCoroutine = StartCoroutine(Blink());
        }
        else if (!playing && isPlaying)
        {
            isPlaying = false;
            StopCoroutine(blinkCoroutine);
            spriteRenderer.enabled = true; // Ensure visible when stopped
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}