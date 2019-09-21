using Core.Mediators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapMessage : Message
{
    public Vector2 SlapPosition { get; set; }
    public int PlayerNumber { get; set; }

    public SlapMessage(object sender, float positionX, float positionY, int playerNumber) : base(sender)
    {
        SlapPosition = new Vector2(positionX, positionY);
        PlayerNumber = playerNumber;
    }
}
