using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasSlappedMessage : Message
{
    public int PlayerNumber { get; set; }

    public WasSlappedMessage(object sender, int playerNumber) : base(sender)
    {
        PlayerNumber = playerNumber;
    }
}
