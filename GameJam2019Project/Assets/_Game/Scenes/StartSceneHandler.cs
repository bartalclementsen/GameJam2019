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

    [SerializeField]
    private GameObject _mainMenuPanel;

    [SerializeField]
    private GameObject _numberOfPlayerPanel;

    [SerializeField]
    private GameObject _readyPanel;

    private void Start()
    {
        _versionText.text = $"{Application.companyName} - {Application.productName} - version: {Application.version}";

        _mainMenuPanel.SetActive(true);
        _numberOfPlayerPanel.SetActive(false);
        _readyPanel.SetActive(false);
    }

    private void Update()
    {
    }

    public void StartNewGame() 
    {
        _mainMenuPanel.SetActive(false);
        _numberOfPlayerPanel.SetActive(true);
    }

    public void StartGame() 
    {
        SceneManager.LoadSceneAsync(1);
    }

     public void SelectNumberOfPlayer(int numberOfPlayers) 
    {
        Game.NumberOfPlayers = numberOfPlayers;
        
        _numberOfPlayerPanel.SetActive(false);
        _readyPanel.SetActive(true);
    }
    
    public void NumberOfPlayersBack() 
    {
        _mainMenuPanel.SetActive(true);
        _numberOfPlayerPanel.SetActive(false);
    }

    public void ReadyBack() 
    {
        _numberOfPlayerPanel.SetActive(true);
        _readyPanel.SetActive(false);
    }


    public void Exit() 
    {
        Application.Quit();
    }
}

