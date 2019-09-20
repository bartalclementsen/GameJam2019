using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LooseSceneHandler : MonoBehaviour
{
   
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void TryAgain() 
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void BackToMainMenu() 
    {
        SceneManager.LoadSceneAsync(0);
    }
}
