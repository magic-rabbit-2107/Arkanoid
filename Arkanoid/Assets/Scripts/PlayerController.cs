using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public int movementSpeed = 10;
    private Rigidbody2D _rigidBody;
    private Ball _currentBall;


    public float leftLimit = -8f;
    public float rightLimit = 8f;
    private bool _inputEnabled = false;

    private Animator _animator;


    private bool _animationCompleted = false;
    
    public float VelocityX { get; private set; }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();



        // Ensure Rigidbody settings
        _rigidBody.gravityScale = 0f;
        _rigidBody.freezeRotation = true;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;

        _animator = GetComponent<Animator>();

        // foreach (var p in _animator.parameters)
        // {
        //     Debug.Log(p.name);
        // }   
    }

    private void Start()
    {
    }


    private void FixedUpdate()
    {
        updateHorizonalMovement();

        VelocityX = _rigidBody.velocity.x;

    }

    private void updateHorizonalMovement()
    {

            if (!_inputEnabled)
            {
                _rigidBody.velocity = new Vector2(0f, _rigidBody.velocity.y);
                return;
            }

            float horizontalMovement = Input.GetAxis("Horizontal");

            _rigidBody.velocity = new Vector2(
                horizontalMovement * movementSpeed,
                _rigidBody.velocity.y
            );
    }

    private void Update()
    {
        onSpacePressed();

    //    float currentX = transform.position.x;
    //     VelocityX = (currentX - _lastX) / Time.deltaTime;
    //     _lastX = currentX;
    }

    private void onSpacePressed()
    {
        if (!_inputEnabled) return;

        bool spacePressed = Input.GetKeyDown(KeyCode.Space);

        if (spacePressed && _currentBall != null)
        {
            if (!_currentBall.IsLaunched)
            {
                _currentBall.Launch();
            }
        }
    }

    public void SetBall(Ball ball)
    {
        _currentBall = ball;
    }

    public void setInput(bool input)
    {
        this._inputEnabled = input;
    }

    public void showPlayerStart()
    {
        _animator.SetTrigger("Start");
        _animator.speed = 2f;
    }

    public void showPlayerDeath()
    {
        _animator.SetTrigger("Destroy");
        // _animator.speed = 2f;
    }

    public void showPlayingState()
    {
        _animator.SetTrigger("Playing");
        _animator.speed = 0.5f;
    }

public bool AnimationStateCompleted(string stateName)
{
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log(stateInfo.IsName(stateName));

    if (!stateInfo.IsName(stateName))
    {
        _animationCompleted = false; // reset when state changes
        return false;
    }

    if (!_animationCompleted && stateInfo.normalizedTime >= 1f && !_animator.IsInTransition(0))
    {
        _animationCompleted = true;
        return true; // triggers only once
    }

    return false;
}

}
