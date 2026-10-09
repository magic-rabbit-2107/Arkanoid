using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private bool _ballLaunched = false;

    public float launchSpeed = 8f;

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
    }

    public void AttachToPaddle(Transform paddle)
    {
        _ballLaunched = false;
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.isKinematic = true;

        transform.SetParent(paddle);
        transform.localPosition = new Vector2(0f, 0.05f);
    }


    public void Launch()
    {
        if (_ballLaunched) return;

        _ballLaunched = true;
        transform.SetParent(null);
        _rigidBody.isKinematic = false;
        

        SetVelocity(Vector2.up);

    }

    public bool IsLaunched => _ballLaunched;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only bounce if we hit something solid
        if (!collision.collider.CompareTag("Block") &&
        !collision.collider.CompareTag("Wall") &&
        !collision.collider.CompareTag("Paddle"))
            return;

        bool isPaddle = collision.collider.CompareTag("Paddle");

        // if (!isPaddle)
        // {
            if (Time.time - _lastBounceTime < BounceCooldown)
                return;

            _lastBounceTime = Time.time;
        //}



        if (collision.collider.CompareTag("Paddle"))
        {
            Debug.Log(
            $"BEFORE PADDLE | " +
            $"velocity={_rigidBody.velocity} | " +
            $"position={_rigidBody.position}"
            );

            BounceFromPaddle(collision);


            Debug.Log(
                $"AFTER PADDLE | " +
                $"velocity={_rigidBody.velocity} | " +
                $"position={_rigidBody.position}"
            );

            return;
        }

        BounceFromSurface(collision);

        if (collision.collider.CompareTag("Block"))
        {
            if (_blockHitSound1 != null)
            {
                _audioSource.PlayOneShot(_blockHitSound1);
            }
        }
    }

    
    private Vector2 ClampDirection(Vector2 dir)
    {
        // Enforce minimum vertical movement
        if (Mathf.Abs(dir.y) < _minVerticalComponent)
            dir.y = _minVerticalComponent * Mathf.Sign(dir.y == 0 ? Random.value - 0.5f : dir.y);

        // Ensure the speed stays above a threshold
        if (dir.magnitude < 0.1f) 
            dir = dir.normalized * 0.1f; // Adjust the threshold value if necessary

        return dir.normalized;
    }


    private void SetVelocity(Vector2 dir)
    {
        _rigidBody.velocity = ClampDirection(dir) * _ballSpeed;
    }

    private void BounceFromPaddle(Collision2D collision)
    {
        Debug.Log(
            $"PADDLE HIT | velocity={_rigidBody.velocity} | normal={collision.GetContact(0).normal}"
        );

        // If the ball is travelling downward,
        // always treat the paddle collision as a top hit.
        if (_rigidBody.velocity.y < 0f)
        {
            BounceFromTopOfPaddle(collision);
            return;
        }

        // Otherwise, handle an actual side collision.
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;

        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            Vector2 direction = new Vector2(
                Mathf.Sign(normal.x),
                -0.35f
            ).normalized;

            SetVelocity(direction);

            PlayPaddleSound();

            return;
        }

        BounceFromTopOfPaddle(collision);
    }

 
    private void BounceFromSurface(Collision2D collision)
    {
        Vector2 dir = _rigidBody.velocity.normalized;
        Vector2 normal = collision.contacts[0].normal;

        // Determine which axis to flip
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            dir.x = Mathf.Sign(normal.x) * Mathf.Abs(dir.x);
        }
        else
        {
            dir.y = Mathf.Sign(normal.y) * Mathf.Abs(dir.y);
        }

        dir.x += Random.Range(-0.01f, 0.01f);
        dir.y += Random.Range(-0.01f, 0.01f);

        if (Mathf.Abs(dir.y) < _minVerticalComponent)
            dir.y = -_minVerticalComponent;

        // Apply minimum speed if the ball's velocity is too low
        // if (_rigidBody.velocity.magnitude < 1f)  // Adjust threshold as needed
        //     _rigidBody.velocity = _rigidBody.velocity.normalized * _ballSpeed;


        SetVelocity(dir);
    }

    

    private void BounceFromTopOfPaddle(Collision2D collision)
    {
        CircleCollider2D ballCollider = GetComponent<CircleCollider2D>();

        Bounds ballBounds = ballCollider.bounds;
        Bounds paddleBounds = collision.collider.bounds;

        // Make sure the ball is completely above the paddle.
        float penetration = paddleBounds.max.y - ballBounds.min.y;

        if (penetration > 0f)
        {
            _rigidBody.position += Vector2.up * (penetration + 0.01f);
        }

        float hitPoint =
            transform.position.x - collision.collider.bounds.center.x;

        float paddleWidth =
            collision.collider.bounds.size.x;

        float normalizedHit =
            Mathf.Clamp(
                hitPoint / (paddleWidth / 2f),
                -1f,
                1f
            );

        float bounceAngle =
            normalizedHit * _maxBounceAngle * Mathf.Deg2Rad;

        Vector2 direction = new Vector2(
            Mathf.Sin(bounceAngle),
            Mathf.Cos(bounceAngle)
        );

        direction.y = Mathf.Abs(direction.y);

        PlayerController paddle =
            collision.collider.GetComponent<PlayerController>();

        if (paddle != null)
        {
            float spin = paddle.VelocityX * _spinStrength;
            spin = Mathf.Clamp(spin, -_maxSpin, _maxSpin);

            direction.x += spin;
        }

        direction = ClampDirection(direction);

        SetVelocity(direction);


        PlayPaddleSound();
    }

    private void PlayPaddleSound()
    {
        if (_paddleHitSound != null && _ballLaunched) 
             _audioSource.PlayOneShot(_paddleHitSound); 
    }

}
