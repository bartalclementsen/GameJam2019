using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
        if(playerController != null) {
            playerController.gameObject.SetActive(false);
            Debug.Log(playerController.gameObject.name + " dies!");
        }
    }
}
