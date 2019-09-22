using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMessage : Message
{
    public int PlayerNumber { get; set; }

    public DeathMessage(object sender, int playerNumber) : base(sender)
    {
        PlayerNumber = playerNumber;
    }
}
