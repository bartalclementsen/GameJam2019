using Core.Loggers;
using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerSkin;
    private PlayerController _playerController;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private IMessenger _messenger;
    private Core.Loggers.ILogger _logger;
    ISubscriptionToken _changeDirectionToken;

    public float scaleSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = _playerSkin.GetComponent<Animator>();
        _playerController = this.GetComponent<PlayerController>();
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _messenger = Game.Container.Resolve<IMessenger>();
        _logger = Game.Container.Resolve<ILoggerFactory>().Create(this);

        _changeDirectionToken = _messenger.Subscribe<ChangeDirectionMessage>((message) =>
        {
            if (message.PlayerNumber == _playerController.playerNumber)
            {
                _logger.Log($"Player {message.PlayerNumber} changed direction.");
                this.transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.z, transform.localScale.z);
            }

        });
    }

    // Update is called once per frame
    void Update()
    {
        var speed = Mathf.Abs(_rigidbody.velocity.x);
        _animator.SetBool("isGrounded", _playerController.isGrounded);
        _animator.SetFloat("speed", speed * scaleSpeed);
    }

    private void OnDestroy()
    {
        _changeDirectionToken.Dispose();
    }
}
