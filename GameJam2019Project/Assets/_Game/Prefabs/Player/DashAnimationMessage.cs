using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAnimationMessage : Message
{
    public int PlayerNumber { get; set; }

    public DashAnimationMessage(object sender, int playerNumber) : base(sender)
    {
        PlayerNumber = playerNumber;
    }
}
