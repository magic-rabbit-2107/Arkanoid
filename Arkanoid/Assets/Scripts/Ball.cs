using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private bool _ballLaunched = false;

    public float launchSpeed = 5;
    private Arena _arena;

    [SerializeField] private AudioClip _paddleHitSound;
    [SerializeField] private AudioClip _blockHitSound1;
    private AudioSource _audioSource;

    [SerializeField] private float _maxBounceAngle = 60f;
    [SerializeField] private float _minVerticalComponent = 0.2f;
    [SerializeField] private float _ballSpeed = 8f;

    [SerializeField] private float _spinStrength = 0.004f;
    [SerializeField] private float _maxSpin = 0.35f;

    private float _lastBounceTime;
    private const float BounceCooldown = 0.02f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        _rigidBody.gravityScale = 0;

        _rigidBody.sharedMaterial = null;

        _arena = FindFirstObjectByType<Arena>();
    }

    
    /// <summary>
    /// Attaches the ball to the paddle before it has been launched.
    /// Resets its velocity and makes it kinematic so it follows the paddle.
    /// Optionally positions the ball centrally above the paddle.
    /// </summary>
    /// <param name="paddle">The paddle Transform the ball will follow.</param>
    /// <param name="launchFromCenter">
    /// If true, positions the ball centrally above the paddle.
    /// </param>
    public void AttachToPaddle(Transform paddle, bool launchFromCenter = true)
    {
        _ballLaunched = false;

        // Stop the ball and disable physics movement while attached.
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.isKinematic = true;

        // Make the ball follow the paddle.
        transform.SetParent(paddle);

        if (launchFromCenter || transform.localPosition.y < 0f)
        {
            Collider2D paddleCollider = paddle.GetComponent<Collider2D>();
            Collider2D ballCollider = GetComponent<Collider2D>();

            // Calculate a position just above the paddle.
            float ballY = paddleCollider.bounds.max.y
                        + ballCollider.bounds.extents.y
                        + 0.1f;

            transform.position = new Vector2(paddle.position.x, ballY);
        }
    }

    /// <summary>
    /// Launches the ball upwards from the paddle.
    /// Prevents the ball from being launched more than once.
    /// </summary>
    public void Launch()
    {
        if (_ballLaunched) return;

        _ballLaunched = true;

        // Detach the ball so it can move independently of the paddle.
        transform.SetParent(null);

        // Re-enable physics simulation and disable gravity.
        _rigidBody.isKinematic = false;
        _rigidBody.gravityScale = 0;

        // Give the ball its initial upward velocity.
        _rigidBody.velocity = Vector2.up * _ballSpeed;
    }

    /// <summary>
    /// Indicates whether the ball has been launched.
    /// </summary>
    public bool IsLaunched => _ballLaunched;

    /// <summary>
    /// Handles collisions between the ball and blocks, walls or paddle.
    /// Calls the appropriate bounce method depending on the collider tag.
    /// </summary>
    /// <param name="collision">Information about the collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collisions with blocks.
        if (collision.collider.CompareTag("Block"))
        {
            if (_blockHitSound1 != null)
            {
                _audioSource.PlayOneShot(_blockHitSound1);
            }

            // Bounce downwards after hitting a block.
            BounceFromSurface(collision, false);
            return;
        }

        // Handle collisions with walls.
        if (collision.collider.CompareTag("Wall"))
        {
            BounceFromWall(collision);
            return;
        }

        // Handle collisions with the paddle.
        if (collision.collider.CompareTag("Paddle"))
        {
            BounceFromPaddle(collision);
            return;
        }
    }

    /// <summary>
    /// Normalizes a direction vector and ensures that neither its
    /// horizontal nor vertical component becomes too small.
    /// Helps prevent the ball from travelling almost horizontally
    /// or almost vertically.
    /// </summary>
    /// <param name="dir">The intended bounce direction.</param>
    /// <returns>A normalized direction with minimum component values.</returns>
    private Vector2 ClampDirection(Vector2 dir)
    {
        dir.Normalize();

        float minComponent = 0.2f;

        // Ensure the horizontal component is not too small.
        if (Mathf.Abs(dir.x) < minComponent)
        {
            dir.x = Mathf.Sign(dir.x == 0 ? 1f : dir.x) * minComponent;
        }

        // Ensure the vertical component is not too small.
        if (Mathf.Abs(dir.y) < minComponent)
        {
            dir.y = Mathf.Sign(dir.y == 0 ? 1f : dir.y) * minComponent;
        }

        return dir.normalized;
    }

    /// <summary>
    /// Applies a bounce direction to the ball while maintaining
    /// its configured movement speed.
    /// </summary>
    /// <param name="dir">The intended direction of movement.</param>
    private void SetVelocity(Vector2 dir)
    {
        _rigidBody.velocity = ClampDirection(dir) * _ballSpeed;
    }

    /// <summary>
    /// Calculates the ball's bounce direction after hitting a block
    /// or the top or underside of a surface.
    /// The horizontal direction depends on where the ball hits the surface.
    /// </summary>
    /// <param name="collision">Collision information for the surface.</param>
    /// <param name="bounceUp">True to bounce upwards, false to bounce downwards.</param>
    private void BounceFromSurface(Collision2D collision, bool bounceUp)
    {
        Transform surface = collision.collider.transform;

        // Calculate the horizontal distance between the ball
        // and the centre of the surface.
        float difference = transform.position.x - surface.position.x;

        // Get half the surface width to normalize the impact position.
        float halfWidth = collision.collider.bounds.extents.x;

        // Convert the impact position into a value between -1 and 1.
        float hitOffset = Mathf.Clamp(
            difference / halfWidth,
            -1f,
            1f
        );

        // Calculate the bounce direction using the impact position.
        Vector2 direction = new Vector2(
            hitOffset,
            bounceUp ? 1f : -1f
        ).normalized;

        // Apply the bounce while maintaining consistent ball speed.
        _rigidBody.velocity = ClampDirection(direction) * _ballSpeed;
    }

    /// <summary>
    /// Reflects the ball's incoming velocity when it hits a wall.
    /// Uses the collision's relative velocity to reconstruct the
    /// incoming velocity for a stationary wall.
    /// </summary>
    /// <param name="collision">Collision information for the wall.</param>
    private void BounceFromWall(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        Vector2 normal = contact.normal;

        // Reconstruct the incoming velocity before collision resolution.
        Vector2 incoming = -collision.relativeVelocity;

        // Avoid attempting to reflect a zero-length velocity.
        if (incoming.sqrMagnitude < 0.01f)
        {
            Debug.LogError("Wall collision with zero incoming velocity!");
            return;
        }

        // Reflect the incoming direction around the wall's contact normal.
        Vector2 outgoing = Vector2.Reflect(incoming, normal);

        // Prevent the ball from travelling almost parallel to either axis.
        float minComponent = 0.2f;

        if (Mathf.Abs(outgoing.x) < minComponent)
        {
            outgoing.x = -normal.x * minComponent;
        }

        if (Mathf.Abs(outgoing.y) < minComponent)
        {
            outgoing.y = (outgoing.y < 0f ? -1f : 1f) * minComponent;
        }

        // Normalize the result and restore the configured ball speed.
        outgoing = outgoing.normalized * _ballSpeed;

        _rigidBody.velocity = outgoing;
    }

    /// <summary>
    /// Determines whether the ball has hit the top, underside or side
    /// of the paddle and selects the appropriate bounce behaviour.
    /// </summary>
    /// <param name="collision">Collision information for the paddle.</param>
    private void BounceFromPaddle(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        // Compare the horizontal and vertical components of the contact normal.
        float horizontal = Mathf.Abs(contact.normal.x);
        float vertical = Mathf.Abs(contact.normal.y);

        if (vertical >= horizontal)
        {
            // A vertical contact normal indicates a top or underside collision.
            bool bounceUp = contact.normal.y > 0f;

            if (bounceUp)
            {
                BounceFromTopOfPaddle(collision);
            }
            else
            {
                BounceFromSurface(collision, bounceUp);
            }
            
        }
        else
        {
            // A horizontal contact normal indicates a side collision.
            BounceFromSideOfPaddle(collision);
        }

        // Play the paddle collision sound.
        PlayPaddleSound();
    }

    /// <summary>
    /// Calculates an outward and upward bounce when the ball hits
    /// either side of the paddle.
    /// Does not manually reposition the ball, allowing Unity's
    /// physics engine to resolve the collision.
    /// </summary>
    /// <param name="collision">Collision information for the paddle.</param>
    private void BounceFromSideOfPaddle(Collision2D collision)
    {
        Collider2D paddleCollider = collision.collider;

        // Determine whether the ball hit the left or right side.
        float side = transform.position.x >= paddleCollider.bounds.center.x
            ? 1f
            : -1f;

        // Create an outward and upward bounce direction.
        Vector2 direction = new Vector2(side, 1f).normalized;

        // Apply the bounce at the configured ball speed.
        _rigidBody.velocity = direction * _ballSpeed;
    }

    /// <summary>
    /// Calculates a controlled bounce angle when the ball hits
    /// the top of the paddle.
    /// The impact position and paddle movement can influence
    /// the resulting direction.
    /// </summary>
    /// <param name="collision">Collision information for the paddle.</param>
    private void BounceFromTopOfPaddle(Collision2D collision)
    {
        Bounds paddleBounds = collision.collider.bounds;

        // Calculate the ball's horizontal impact position relative to the paddle.
        float hitPoint = transform.position.x - paddleBounds.center.x;

        float paddleWidth = paddleBounds.size.x;

        // Normalize the impact position between -1 and 1.
        float normalizedHit = Mathf.Clamp(
            hitPoint / (paddleWidth / 2f),
            -1f,
            1f
        );

        // Convert the impact position into a bounce angle.
        float bounceAngle =
            normalizedHit * _maxBounceAngle * Mathf.Deg2Rad;

        // Calculate the initial bounce direction.
        Vector2 direction = new Vector2(
            Mathf.Sin(bounceAngle),
            Mathf.Cos(bounceAngle)
        );

        // Ensure the ball travels upwards.
        direction.y = Mathf.Abs(direction.y);

        // Add horizontal influence from paddle movement.
        PlayerController paddle =
            collision.collider.GetComponent<PlayerController>();

        if (paddle != null)
        {
            float spin = paddle.VelocityX * _spinStrength;
            spin = Mathf.Clamp(spin, -_maxSpin, _maxSpin);

            direction.x += spin;
        }

        // Clamp the direction and apply the configured ball speed.
        SetVelocity(direction);

        PlayPaddleSound();
    }

    /// <summary>
    /// Plays the paddle collision sound if an audio clip is assigned
    /// and the ball has already been launched.
    /// </summary>
    private void PlayPaddleSound()
    {
        if (_paddleHitSound != null && _ballLaunched)
        {
            _audioSource.PlayOneShot(_paddleHitSound);
        }
    }

    /// <summary>
    /// Runs during each physics update while the ball is launched.
    /// Checks for an unexpected loss of velocity.
    /// </summary>
    private void FixedUpdate()
    {
        if (!_ballLaunched)
            return;

        // Detect whether the ball has unexpectedly stopped moving.
        if (_rigidBody.velocity.sqrMagnitude < 0.01f)
        {
            Debug.LogError(
                $"BALL STOPPED! " +
                $"Position: {_rigidBody.position}, " +
                $"Velocity: {_rigidBody.velocity}, " +
                $"Kinematic: {_rigidBody.isKinematic}, " +
                $"Simulated: {_rigidBody.simulated}, " +
                $"Constraints: {_rigidBody.constraints}"
            );
        }
    }

}
