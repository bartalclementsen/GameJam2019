using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMessage : Message
{
    public int PlayerNumber { get; set; }

    public JumpMessage(object sender, int playerNumber) : base(sender)
    {
        PlayerNumber = playerNumber;
    }
}
