using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneHandler : MonoBehaviour
{
    [SerializeField]
    private string _teamName;

    [SerializeField]
    private Text _versionText;

    private void Start()
    {
        _versionText.text = $"{Application.companyName} - {Application.productName} - version: {Application.version}";
    }

    private void Update()
    {
        Debug.Log($"Player1Horizontal: {Input.GetAxis("Player1Horizontal")}");
        Debug.Log($"Player1Vertical: {Input.GetAxis("Player1Vertical")}");
        Debug.Log($"Player1Fire1: {Input.GetAxis("Player1Fire1")}");
        Debug.Log($"Player1Fire2: {Input.GetAxis("Player1Fire2")}");
        Debug.Log($"Player1Jump: {Input.GetAxis("Player1Jump")}");
        Debug.Log($"Player1Submit: {Input.GetAxis("Player1Submit")}");
        Debug.Log($"Player1Cancel: {Input.GetAxis("Player1Cancel")}");
        Debug.Log($"Player1Pause: {Input.GetAxis("Player1Pause")}");
    }

    public void StartNewGame() 
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Exit() 
    {
        Application.Quit();
    }
}
