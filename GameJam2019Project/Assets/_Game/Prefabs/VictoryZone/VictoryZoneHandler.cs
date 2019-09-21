using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryZoneHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    }
}
