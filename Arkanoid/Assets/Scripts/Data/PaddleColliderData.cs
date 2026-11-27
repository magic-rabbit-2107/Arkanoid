using UnityEngine;

/// <summary>
/// Stores the initial dimensions, offset, and bounds of a paddle's
/// <see cref="BoxCollider2D"/>.
/// </summary>
[System.Serializable]
public class PaddleColliderData
{
    /// <summary>
    /// Gets or sets the initial size of the paddle's collider.
    /// </summary>
    public Vector2 InitialSize;

    /// <summary>
    /// Gets or sets the initial local offset of the paddle's collider.
    /// </summary>
    public Vector2 InitialOffset;

    /// <summary>
    /// Gets or sets the initial world-space bounds of the paddle's collider.
    /// </summary>
    public Bounds InitialBounds;

    /// <summary>
    /// Initializes a new instance of <see cref="PaddleColliderData"/>
    /// using the current properties of the specified collider.
    /// </summary>
    /// <param name="collider">
    /// The <see cref="BoxCollider2D"/> whose initial properties will be stored.
    /// </param>
    public PaddleColliderData(BoxCollider2D collider)
    {
        InitialSize = collider.size;
        InitialOffset = collider.offset;
        InitialBounds = collider.bounds;
    }
}