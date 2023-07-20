using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelGoalSetting
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}
public class GoalManager : MonoBehaviour
{
    public LevelGoalSetting[] levelGoalSettings;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    private Board board;
    private EndGameManager endGameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        endGameManager = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupGoals();
    }

    void GetGoals()
    {
        if (board != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world != null)
                {
                    if (board.world.levels[board.level] != null)
                    {
                        levelGoalSettings = board.world.levels[board.level].levelGoalSettings;
                        for (int i = 0; i < levelGoalSettings.Length; i++)
                        {
                            levelGoalSettings[i].numberCollected = 0;
                        }
                    }
                }

            }
        }
    }
    void SetupGoals()
    {
        for (int i = 0; i < levelGoalSettings.Length; i++)
        {
            // Create a new Goal Panel at the goalIntroParent position
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            // Set the image and text of the goal;
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoalSettings[i].goalSprite;
            panel.thisString = "0/" + levelGoalSettings[i].numberNeeded;

            // Create a new goal Panel at the goalGameParent position
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoalSettings[i].goalSprite;
            panel.thisString = "0/" + levelGoalSettings[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoalSettings.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoalSettings[i].numberCollected +"/" +levelGoalSettings[i].numberNeeded;
            if (levelGoalSettings[i].numberCollected >= levelGoalSettings[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoalSettings[i].numberNeeded + "/" + levelGoalSettings[i].numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoalSettings.Length)
        {
            if (endGameManager != null)
            {
                endGameManager.WinGame();
            }
            
        }

    }

    public void CompareGoal (string goalToCompare)
    {
        for (int i = 0; i < levelGoalSettings.Length; i++)
        {
            if (goalToCompare == levelGoalSettings[i].matchValue)
            {
                levelGoalSettings[i].numberCollected++;
            }
        }
    }
}
