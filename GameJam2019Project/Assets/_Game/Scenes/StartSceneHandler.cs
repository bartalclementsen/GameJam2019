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
        _versionText.text = $"{Application.companyName} - {Application.productName} - {Application.version}";
    }

    private void Update()
    {
        
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
