using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this controls the animation on the canvas that works like a Fade in/out panel. Contains a method for the GameInfoPanel ok button
public class FadePanelController : MonoBehaviour
{

    public Animator panelAnim;
    public Animator gameInfoAnim;
    public float gameStartDelay = 1f;

    public void OK() // method for the ok button
    {
        if (panelAnim != null && gameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStartCo());

        } 
    }

    public void SetGameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);

    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(gameStartDelay);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }


}
