using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="World",menuName ="Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int width;
    public int height;

    [Header("Starting Tiles")]
    public TileProperties[] boardLayout;

    [Header("Available Gems")]
    public GameObject[] gems;

    [Header("Score Goals")]
    public int[] scoreGoals;

    [Header("End Game Requirements")]
    public GameModeSetting gameModeSetting;
    public LevelGoalSetting[] levelGoalSettings;

}
