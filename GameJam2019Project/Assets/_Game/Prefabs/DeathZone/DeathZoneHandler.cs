using Core.Loggers;
using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneHandler : MonoBehaviour
{
    private Core.Loggers.ILogger _logger;
    private IMessenger _messenger;

    void Start()
    {
        _logger = Game.Container.Resolve<ILoggerFactory>().Create(this);
        _messenger = Game.Container.Resolve<IMessenger>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
        if(playerController != null) {
            playerController.gameObject.SetActive(false);
            _messenger.Publish(new DeathMessage(this, playerController.playerNumber));
            _logger.Log(playerController.gameObject.name + " dies!");
        }
    }
}
