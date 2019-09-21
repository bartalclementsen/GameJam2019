using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var playerController = transform.parent.GetComponent<PlayerController>();
        var textMesh = transform.GetComponent<TextMeshPro>();
        textMesh.text = $"Player {playerController.playerNumber}";
        textMesh.faceColor = ColorFloatToByte(playerController.playerColor);
    }

    private Color32 ColorFloatToByte(Color col)
    {
        return new Color32((byte)Mathf.FloorToInt(col.r * 255f), (byte)Mathf.FloorToInt(col.g * 255f), (byte)Mathf.FloorToInt(col.b * 255f), 255);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
