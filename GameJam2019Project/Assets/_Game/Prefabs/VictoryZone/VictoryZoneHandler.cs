using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMessage : Core.Mediators.IMessage
{
    public object Sender { get; }

    public PlayerController VictoriousPlayerController { get; }

    public VictoryMessage(object sender, PlayerController victoriousPlayerController)
    {
        VictoriousPlayerController = victoriousPlayerController;
    }
}

public class VictoryZoneHandler : MonoBehaviour
{
    private Core.Mediators.IMessenger _messenger;
    private void Start() {
        _messenger = Game.Container.Resolve<Core.Mediators.IMessenger>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
        if(playerController != null) {
            _messenger.Publish(new VictoryMessage(this, playerController));
            Debug.Log(playerController.gameObject.name + " wins!");
        }
    }
}
