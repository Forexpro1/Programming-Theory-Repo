using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameMode
{
    Moves,
    Time,
    Battle
}

[System.Serializable]
public class GameModeSetting
{
    public GameMode gameMode;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public TMP_Text counter; 
    public GameModeSetting gameModeSetting;
    public int currentCounterValue;
    private Board board;
    private FadePanelController fadePanelController;
    private float timerSeconds;
    
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        fadePanelController = FindObjectOfType<FadePanelController>();
        SetGameMode();
        SetupGame();
        
    }

    void SetGameMode()
    {
        if (board.world !=null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    gameModeSetting = board.world.levels[board.level].gameModeSetting;

                }

            }
        }
        
    }

    void SetupGame()
    {
        currentCounterValue = gameModeSetting.counterValue;
        if (gameModeSetting.gameMode == GameMode.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
         
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        fadePanelController.SetGameOver();
    }
    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        fadePanelController.SetGameOver();
    }
    // Update is called once per frame
    void Update()
    {
        if (gameModeSetting.gameMode == GameMode.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
