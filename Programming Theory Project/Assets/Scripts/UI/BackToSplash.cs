using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Used to add functions to the EndGame win/lose panels and their corresponding buttons

public class BackToSplash : MonoBehaviour
{
    public string sceneToLoad;
    private GameData gameData;
    private Board board;
    bool maxLevelReached = false;
    public int maxLevel;

    public void WinOK()
    {
        if (gameData != null)
        {
            gameData.Save();
            if (gameData.saveData.isActive[maxLevel-1] )
            {
                maxLevelReached = true;
                //Debug.Log("The Maximum Level For this Demo Has been reach. Thank you for playing my game.");
            }

            if (!maxLevelReached)
            {
                gameData.saveData.isActive[board.level + 1] = true;
            }
            
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoseOK()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();

    }
    
}
