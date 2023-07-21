using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// needs to be implement still for additional players

public enum GemOwner
{
    player1,
    player2,
    npc
}
[System.Serializable]
public class Player
{
    public GemOwner gemOwner;
    public float health;

}

public class PlayerManager : MonoBehaviour
{

    public int numberOfPlayers = 2;
    public Player[] players;
    // Start is called before the first frame update
    private void Awake()
    {
        players = new Player[numberOfPlayers];
        
       
        
    }
    private void Start()
    {
        //SetupPlayers();

    }
    void SetupPlayers()
    {
        Debug.Log("Setup complete");
        players[0].health = 100f;
        players[0].gemOwner = GemOwner.player1;
        players[1].health = 100f;
        players[1].gemOwner = GemOwner.player2; 
    } 
}
