using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirectionMessage : IMessage
{
    public object Sender { get; set; }
    public int PlayerNumber { get; set; }

    public ChangeDirectionMessage(object sender, int playerNumber)
    {
        Sender = sender;
        PlayerNumber = playerNumber;
    }
}
