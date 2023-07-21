using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// used for the Title Screen and button functions

public class GameStartManager : MonoBehaviour
{
   
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level Select", LoadSceneMode.Single);
    }

    public void Home()
    {
        SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
    }
    public void Exit()
    {
       Application.Quit();
    }

}
