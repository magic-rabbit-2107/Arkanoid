using UnityEngine;

[ExecuteAlways]
public class ResponsiveLevelScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform levelRoot;
    [SerializeField] private SpriteRenderer referenceSprite;

    [Header("Scaling Settings")]
    [SerializeField] private float aspectCorrection = 1.07f;
    [SerializeField] private bool autoScaleOnStart = true;
    [SerializeField] private bool autoUpdateInEditor = true;

    private Vector2 _lastScreenSize;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Start()
    {
        if (autoScaleOnStart)
            Scale();
        
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!autoUpdateInEditor) return;

        if (ScreenSizeChanged())
            Scale();
    }
#else
    private void Update()
    {
        if (ScreenSizeChanged())
            Scale();
    }
#endif

    public void Scale()
    {
        if (!ValidateReferences())
            return;

        Vector2 camSize = GetCameraWorldSize(targetCamera);
        Vector2 spriteSize = referenceSprite.bounds.size;

        bool isPortrait = camSize.y > camSize.x;

        float widthMultiplier;
        float heightMultiplier;

        if (isPortrait)
        {
            widthMultiplier = camSize.x / spriteSize.x;
            heightMultiplier = widthMultiplier * aspectCorrection;
        }
        else
        {
            heightMultiplier = camSize.y / spriteSize.y;
            widthMultiplier = heightMultiplier / aspectCorrection;
        }

        levelRoot.localScale = new Vector3(widthMultiplier, heightMultiplier, 1f);
    }

    private Vector2 GetCameraWorldSize(Camera cam)
    {
        if (!cam.orthographic)
        {
            Debug.LogError("ResponsiveLevelScaler requires an orthographic camera.");
            return Vector2.zero;
        }

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        return new Vector2(width, height);
    }

    private bool ValidateReferences()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("Target Camera not assigned.");
            return false;
        }

        if (levelRoot == null)
        {
            Debug.LogWarning("Level Root not assigned.");
            return false;
        }

        if (referenceSprite == null)
        {
            Debug.LogWarning("Reference Sprite not assigned.");
            return false;
        }

        return true;
    }

    private bool ScreenSizeChanged()
    {
        if (_lastScreenSize.x != Screen.width || 
            _lastScreenSize.y != Screen.height)
        {
            _lastScreenSize = new Vector2(Screen.width, Screen.height);
            return true;
        }

        return false;
    }
}