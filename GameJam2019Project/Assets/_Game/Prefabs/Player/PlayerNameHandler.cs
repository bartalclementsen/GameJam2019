using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject _dashes;

    private PlayerController _playerController;

    private int _currentDashesUsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();

        TextMeshPro textMesh = transform.GetComponent<TextMeshPro>();
        textMesh.text = $"Player {_playerController.playerNumber}";
        textMesh.faceColor = ColorFloatToByte(_playerController.playerColor);

        TextMeshPro dashes = _dashes.GetComponent<TextMeshPro>();
        dashes.faceColor = ColorFloatToByte(_playerController.playerColor);
    }

    private Color32 ColorFloatToByte(Color col)
    {
        return new Color32((byte)Mathf.FloorToInt(col.r * 255f), (byte)Mathf.FloorToInt(col.g * 255f), (byte)Mathf.FloorToInt(col.b * 255f), 255);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController.dashesUsed > _currentDashesUsed)
        {
            string stars = "";
            for (int i = _currentDashesUsed; i < _playerController.maxDashes - 1; i++)
            {
                stars += "*";
            }

            TextMeshPro textMesh = _dashes.GetComponent<TextMeshPro>();
            textMesh.SetText(stars);

            _currentDashesUsed++;
        }
    }
}
