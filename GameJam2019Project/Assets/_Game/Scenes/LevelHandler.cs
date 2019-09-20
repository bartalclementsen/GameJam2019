using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{   
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void WinGame() 
    {
        SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 2);
    }

    public void LooseGame() 
    {
        SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 1);
    }
}
