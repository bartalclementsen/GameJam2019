using Core.Loggers;
using Core.Mediators;
using System;
using UnityEngine;
using ILogger = Core.Loggers.ILogger;

public class PlayerController : MonoBehaviour
{
    public int playerNumber = 1;
    public Color playerColor = Color.white;
    public float moveSpeed = 5f;
    public float jumpPower = 7f;
    public int maxDashes = 3;
    public int dashesUsed = 0;

    public float slapRange = 0.75f;
    public float slapRadius = 0.25f;


    public bool debugEnabled = false;

    public Vector3 rayCenterOffset;
    public float groundRayDistance = 0.48f;
    public float groundRayAngle = 45f;
    public int groundRayNumber = 9;

    public bool canMakeActions = true;

    public LayerMask groundLayers;

    public bool isGrounded;
    private Rigidbody2D _body;
    private FaceDirection _faceDirection = FaceDirection.Right;
    private FaceDirection _dashingDirection;


    // Dash
    public DashState dashState;
    public float dashTimer;
    public float dashSpeed = 3f;
    public float maxDash = 20f;

    // Slap
    public int slapsUsed = 0;
    public int maxSlaps = 3;
    private bool _slapped;
    private bool _isSlappingLocked = false;
    private float _slapTimer = 0;

    public Vector2 savedVelocity;

    private ILogger _logger;
    private IMessenger _messenger;
    private ISubscriptionToken _slapMessageToken;


    void Start()
    {
        _body = GetComponent<Rigidbody2D>();

        _logger = Game.Container.Resolve<ILoggerFactory>().Create(this);
        _messenger = Game.Container.Resolve<IMessenger>();

        _slapMessageToken = _messenger.Subscribe<SlapMessage>((message) =>
        {
            HandleSlapped(message);      
        });

    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        var down = -Vector2.up;
        for (float f = -0.5f * groundRayAngle; f <= 0.5f * groundRayAngle; f += groundRayAngle / groundRayNumber)
        {
            var direction = Quaternion.Euler(0, 0, f) * down;
            Gizmos.DrawRay(transform.position + rayCenterOffset, direction * groundRayDistance);
        }

        Vector2 slapperHitPosition = new Vector2(transform.position.x + slapRange, transform.position.y);
        Gizmos.DrawWireSphere(new Vector3(slapperHitPosition.x, slapperHitPosition.y, 0f), slapRadius);


    }

    void FixedUpdate()
    {
        Vector2 down = -Vector2.up;
        bool didHit = false;
        for (float f = -0.5f * groundRayAngle; f <= 0.5f * groundRayAngle; f += groundRayAngle / groundRayNumber)
        {
            var direction = Quaternion.Euler(0, 0, f) * down;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + rayCenterOffset, direction, groundRayDistance, groundLayers);

            didHit = didHit || (hit.collider != null);
        }
        isGrounded = didHit;

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

        Vector2 newVelocity = _body.velocity;
        newVelocity = Move(moveHorizontal, moveVertical);

        // Slap
        if (isSlapping)
            Slap();

        // Sprint / Slap
        if (_slapped)
        {
            savedVelocity = new Vector2(1, 0);
            _slapped = false;
        }

        newVelocity.x = HandleDash(newVelocity.x, isSprinting);
        _body.velocity = newVelocity;

        // Jump (only if grounded)
        if (isJumping && isGrounded)
            Jump();
    }

    private Vector2 Move(float moveHorizontal, float moveVertical)
    {
        var oldDirection = _faceDirection;

        if (Mathf.Abs(moveHorizontal) > 0.1)
        {
            if (moveHorizontal > 0)
                _faceDirection = FaceDirection.Right;
            else
                _faceDirection = FaceDirection.Left;
        }

        if(oldDirection != _faceDirection)
        {
            _messenger.Publish(new ChangeDirectionMessage(this, playerNumber));
        }

        return new Vector2(moveHorizontal * moveSpeed, _body.velocity.y);
    }

    private void Jump()
    {
        _messenger.Publish(new JumpMessage(this, this.playerNumber));
        _body.velocity = new Vector2(_body.velocity.x, jumpPower);
    }

    private float HandleDash(float velocityX, bool isSprinting)
    {
        float resultSpeed = velocityX;

        switch (dashState)
        {
            case DashState.Ready:
                if (isSprinting && dashesUsed < maxDashes)
                {
                    _messenger.Publish(new DashAnimationMessage(this, playerNumber));

                    dashesUsed++;

                    savedVelocity = _body.velocity;

                    if (_faceDirection == FaceDirection.Right)
                    {
                        resultSpeed = Math.Abs(velocityX) * dashSpeed;
                        _dashingDirection = FaceDirection.Right;
                    }
                    else
                    {
                        resultSpeed = -Math.Abs(velocityX) * dashSpeed;
                        _dashingDirection = FaceDirection.Left;
                    }

                    dashState = DashState.Dashing;
                }
                break;
            case DashState.Dashing:
                dashTimer += Time.fixedDeltaTime * 3;

                if (_dashingDirection == FaceDirection.Right)
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
        if (slapsUsed >= maxSlaps)
            return;

        if (!_isSlappingLocked)
        {
            _messenger.Publish(new SlapMessage(this, transform.position.x, transform.position.y, playerNumber));

            slapsUsed++;

            _isSlappingLocked = true;
            _slapTimer = Time.fixedDeltaTime + 0.5f;
        }

        if (_slapTimer <= 0)
        {
            _isSlappingLocked = false;
        }

        _slapTimer -= Time.fixedDeltaTime;
    }

    private void HandleSlapped(SlapMessage message)
    {
        if (message.PlayerNumber == playerNumber)
        {
            _logger.Log($"I slapped at position: ({message.SlapPosition.x}, {message.SlapPosition.y})");
            return;
        }

        Vector2 slapperPosition = message.SlapPosition;
        Vector2 slapperHitPosition = new Vector2(slapperPosition.x + slapRange, slapperPosition.y);
        Vector3 myPosition = transform.position;



        if (Vector2.Distance(myPosition, slapperHitPosition) <= slapRadius)
        {
            _messenger.Publish(new WasSlappedMessage(this, playerNumber));

            _slapped = true;
            dashTimer = 0;
            dashState = DashState.Dashing;
            _dashingDirection = FaceDirection.Left;
        }

    }

    private void OnDestroy()
    {
        _slapMessageToken?.Dispose();
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
