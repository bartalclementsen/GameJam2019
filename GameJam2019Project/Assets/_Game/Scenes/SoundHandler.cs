using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private IMessenger _messenger;
    private ISubscriptionToken _slappedToken;
    private ISubscriptionToken _missToken;
    private ISubscriptionToken _jumpToken;
    private ISubscriptionToken _dashToken;
    private ISubscriptionToken _deathToken;


    [SerializeField]
    private AudioSource _slappedSource;
    [SerializeField]
    private AudioSource _missSource;
    [SerializeField]
    private AudioSource _jumpSource;
    [SerializeField]
    private AudioSource _dashSource;
    [SerializeField]
    private AudioSource _deathSource;

    [SerializeField]
    private List<AudioClip> _jumpSounds;

    void Start()
    {
        _messenger = Game.Container.Resolve<IMessenger>();

        _slappedToken = _messenger?.Subscribe<WasSlappedMessage>((message) =>
        {
            _slappedSource?.PlayOneShot(_slappedSource.clip);
        });

        _missToken = _messenger?.Subscribe<SlapMessage>((message) =>
        {
            _missSource?.PlayOneShot(_missSource.clip);
        });

        _jumpToken = _messenger?.Subscribe<JumpMessage>((message) =>
        {
            _jumpSource?.PlayOneShot(_jumpSounds[message.PlayerNumber % _jumpSounds.Count]);
        });

        _dashToken = _messenger?.Subscribe<DashAnimationMessage>((message) =>
        {
            _dashSource?.PlayOneShot(_dashSource.clip);
        });

        _deathToken = _messenger?.Subscribe<DeathMessage>((message) =>
        {
            _deathSource?.PlayOneShot(_deathSource.clip);
        });
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        _slappedToken?.Dispose();
        _missToken?.Dispose();
        _jumpToken?.Dispose();
        _dashToken?.Dispose();
        _deathToken?.Dispose();
    }
}
