using Core.Loggers;
using Core.Mediators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILogger = Core.Loggers.ILogger;

public class PlayerController : MonoBehaviour
{
    public int playerNumber = 1;
    public float moveSpeed = 5f;
    public float jumpPower = 5f;
    public bool debugEnabled = false;

    public bool canMakeActions = true;

    public LayerMask groundLayers;

    private bool _isGrounded;
    private Rigidbody2D _body;
    private CircleCollider2D _circleCollider2d;
    private FaceDirection _faceDirection = FaceDirection.Right;

    // Dash
    public DashState dashState;
    public float dashTimer;
    public float dashSpeed = 3f;
    public float maxDash = 20f;

    public Vector2 savedVelocity;

    private ILogger _logger;
    private IMessenger _messenger;


    void Start()
    {
        _body = GetComponent<Rigidbody2D> ();
        _circleCollider2d = GetComponent<CircleCollider2D>();

        _logger = Game.Container.Resolve<ILoggerFactory>().Create(this);
        _messenger = Game.Container.Resolve<IMessenger>();

        _messenger.Subscribe<SlapMessage>((message) =>
        {
            if (message.PlayerNumber == playerNumber)
            {
                _logger.Log($"I slapped at position: ({message.SlapPosition.x}, {message.SlapPosition.y})");
            } else
            {
                HandleSlapped(message.SlapPosition);
            }
        });

    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(new Vector2(transform.position.x, transform.position.y - 0.505f), new Vector2(1, 0.1f));
    }

    void FixedUpdate()
    {
        if (!canMakeActions)
            return;

        float moveHorizontal = Input.GetAxis ($"Player{playerNumber}Horizontal");
        float moveVertical = Input.GetAxis ($"Player{playerNumber}Vertical");
        bool isJumping = Input.GetAxis($"Player{playerNumber}Jump") > 0;
        bool isSlapping = Input.GetAxis($"Player{playerNumber}Fire1") > 0;
        bool isSprinting = Input.GetAxis($"Player{playerNumber}Fire2") > 0;

        if (debugEnabled)
        {
            if (moveHorizontal > 0)
                _logger.Log("Right");
            if (moveHorizontal < 0)
                _logger.Log("Left");
            if (moveVertical > 0)
                _logger.Log("Up");
            if (moveVertical < 0)
                _logger.Log("Down");
            if (isJumping)
                _logger.Log("Jump");
            if (isSlapping)
                _logger.Log("Slap!");
            if (isSprinting)
                _logger.Log("Sprint!");
        }

        _isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), _circleCollider2d.radius * transform.localScale.x, groundLayers);

        Vector2 newVelocity = _body.velocity;
        newVelocity = Move(moveHorizontal, moveVertical);

        // Slap
        if (isSlapping)
            Slap();

        // Sprint
        newVelocity.x = HandleDash(newVelocity.x, isSprinting);
        _body.velocity = newVelocity;

        // Jump (only if grounded)
        if (isJumping && _isGrounded)
            Jump();
    }

    private Vector2 Move(float moveHorizontal, float moveVertical)
    {
        if (moveHorizontal > 0)
            _faceDirection = FaceDirection.Right;
        else
            _faceDirection = FaceDirection.Left;

        return new Vector2(moveHorizontal * moveSpeed, _body.velocity.y);
    }

    private void Jump()
    {
        _body.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
    }

    private float HandleDash(float velocityX, bool isSprinting)
    {
        float resultSpeed = velocityX;

        switch (dashState)
        {
            case DashState.Ready:
                if (isSprinting)
                {
                    savedVelocity = _body.velocity;

                    if (_faceDirection == FaceDirection.Right)
                        resultSpeed = Math.Abs(velocityX) * dashSpeed;
                    else
                        resultSpeed = -Math.Abs(velocityX) * dashSpeed;

                    dashState = DashState.Dashing;
                }
                break;
            case DashState.Dashing:
                dashTimer += Time.fixedDeltaTime * 3;

                if (_faceDirection == FaceDirection.Right)
                    resultSpeed = Math.Abs(savedVelocity.x) * dashSpeed;
                else
                    resultSpeed = -Math.Abs(savedVelocity.x) * dashSpeed;

                if (dashTimer >= maxDash)
                {
                    dashTimer = maxDash;
                    resultSpeed = savedVelocity.x;
                    dashState = DashState.Cooldown;
                }
                break;
            case DashState.Cooldown:
                dashTimer -= Time.fixedDeltaTime;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                }
                break;
        }

        return resultSpeed;
    }

    private void Slap()
    {
        _messenger.Publish(new SlapMessage(this, transform.position.x, transform.position.y, playerNumber));
    }

    private void HandleSlapped(Vector2 position)
    {
        _logger.Log($"Someone slapped at position: ({position.x}, {position.y})");
    }
}

public enum DashState
{
    Ready,
    Dashing,
    Cooldown
}

public enum FaceDirection
{
    Left,
    Right
}
