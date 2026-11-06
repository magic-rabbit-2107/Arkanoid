using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Unity objects")]
    // [SerializeField] private Rigidbody2D _ball;
    [SerializeField] private Transform _paddle;

    [Header("Game parameters")]
    [SerializeField] private float _paddleSpeed = 4f;
    [SerializeField] private Vector2 initialBallVelocity = new(2, 4);

    [SerializeField] private float _initialBrickPosition = -0.95f;

    
    private Vector2 _movement;

    private float _playAreaMinXPos = -3.2f;

    private float _playAreaMaximumXPos = 3.2f;

    private bool _isBallInPlay = false;

    private bool _fireButtonPressed = false;

    

    public void Move(InputAction.CallbackContext context) {
        _movement = context.ReadValue<Vector2>();
    }

    public void Fire(InputAction.CallbackContext context) {
        _fireButtonPressed = context.action.triggered && context.action.ReadValue<float>() > 0 & context.action.phase == InputActionPhase.Performed;
    }


    
    private void Update()
    {
        // float moveAmount = _movement.x * _paddleSpeed * Time.deltaTime;

        // _paddle.Translate(moveAmount * Vector2.right);

        // if (_paddle.position.x < _playAreaMinXPos) {
        //     _paddle.position = new Vector2(_playAreaMinXPos, _paddle.position.y);
        // } else if (_paddle.position.x > _playAreaMaximumXPos) {
        //     _paddle.position = new Vector2(_playAreaMaximumXPos, _paddle.position.y);
        // }

        // if (!_isBallInPlay)
        // {
        //     // _ball.transform.position = _paddle.position + (Vector3.up * 0.2f);

        //     if (_fireButtonPressed)
        //     {
        //         _isBallInPlay = true;
        //         // _ball.AddForce(initialBallVelocity, ForceMode2D.Impulse);
        //     }

        // }
    }
    
}
