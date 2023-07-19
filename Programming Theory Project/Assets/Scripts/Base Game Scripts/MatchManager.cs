using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
    public Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    public float findAllMatchesDelay = .2f;
    

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>(); 
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }
    private List<GameObject> IsAdjacentBomb(Gem dot1, Gem dot2, Gem dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.Column, dot1.Row));
        }
        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.Column, dot2.Row));
        }
        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.Column, dot3.Row));
        }
        return currentDots;
    }
    private List<GameObject> IsRowBomb(Gem dot1, Gem dot2, Gem dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.Row));
            board.BombRow(dot1.Row);
        }
        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.Row));
            board.BombRow(dot2.Row);
        }
        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.Row));
            board.BombRow(dot3.Row);
        }
        return currentDots;
    }
    private List<GameObject> IsColumnBomb(Gem dot1, Gem dot2, Gem dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.Column));
            board.BombColumn(dot1.Column);
        }
        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.Column));
            board.BombColumn(dot2.Column);
        }
        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.Column));
            board.BombColumn(dot3.Column);
        }
        return currentDots;
    }
    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Gem>().isMatched = true;
    }
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
    private IEnumerator FindAllMatchesCo()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentGameObject = board.playingBoard[i, j];
               
                if (currentGameObject != null)
                {

                    Gem currentGameObjectDot = currentGameObject.GetComponent<Gem>();
                    if (i > 0 && i < board.width-1)
                    {
                        GameObject leftGameObject = board.playingBoard[i - 1, j];
                        GameObject rightGameObject = board.playingBoard[i + 1, j];
                        if (leftGameObject != null && rightGameObject != null)
                        {
                            Gem rightGameObjectDot = rightGameObject.GetComponent<Gem>();
                            Gem leftGameObjectDot = leftGameObject.GetComponent<Gem>();
                            
                            if (leftGameObject.tag == currentGameObject.tag && rightGameObject.tag == currentGameObject.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftGameObjectDot, currentGameObjectDot, rightGameObjectDot));
                                currentMatches.Union(IsColumnBomb(leftGameObjectDot, currentGameObjectDot, rightGameObjectDot));
                                currentMatches.Union(IsAdjacentBomb(leftGameObjectDot, currentGameObjectDot, rightGameObjectDot));
                               // Debug.Log("tags matched");
                                GetNearbyPieces(leftGameObject, currentGameObject, rightGameObject);  
                            }  
                        }    
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upwardGameObject = board.playingBoard[i, j + 1];

                        GameObject downwardGameObject = board.playingBoard[i, j - 1];
                        if (upwardGameObject != null && downwardGameObject != null)
                        {
                            Gem downDot = downwardGameObject.GetComponent<Gem>();
                            Gem upDot = upwardGameObject.GetComponent<Gem>();

                            if (upwardGameObject.tag == currentGameObject.tag && downwardGameObject.tag == currentGameObject.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDot, currentGameObjectDot, downDot));
                                currentMatches.Union(IsRowBomb(upDot, currentGameObjectDot, downDot));
                                currentMatches.Union(IsAdjacentBomb(upDot, currentGameObjectDot, downDot));
                                GetNearbyPieces(upwardGameObject, currentGameObject, downwardGameObject);
                            } 
                        }
                    }
                }
            }
        }
        yield return null;
    }
    public void MatchPiecesOfColor (string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //check if that piece exist
                if (board.playingBoard[i,j] != null)
                {
                    //check tag on that do
                    if (board.playingBoard[i,j].tag == color)
                    {
                        //set that dot to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = true;
                    }
                }
            }
        }
    }
    public void MakeOneColorRowColumnOrAdjacent(string color, string type)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //check if that piece exist
                if (board.playingBoard[i, j] != null)
                {
                    //check tag on that do
                    if (board.playingBoard[i, j].tag == color  && type == "Row")
                    {
                        //set that dot to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                        board.playingBoard[i, j].GetComponent<Gem>().MakeRowBomb();
                    }
                    if (board.playingBoard[i, j].tag == color && type == "Column")
                    {
                        //set that dot to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                        board.playingBoard[i, j].GetComponent<Gem>().MakeColumnBomb();
                    }
                    if (board.playingBoard[i, j].tag == color && type == "Adjacent")
                    {
                        //set that dot to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                        board.playingBoard[i, j].GetComponent<Gem>().MakeAdjacentBomb();
                    }
                }
            }
        }
    }
    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column+1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //checked if piece is inside board
                if (i >= 0 && i < board.width && j >= 0 && j< board.height)
                {
                    if (board.playingBoard[i,j] != null)
                    {
                        Gem dot = board.playingBoard[i, j].GetComponent<Gem>();
                        if (dot.isRowBomb)
                        {
                            dots.Union(GetRowPieces(i)).ToList();
                        }
                        if (dot.isColorBomb)
                        {
                            MatchPiecesOfColor(board.gemPrefabsArray[Random.Range(0, board.gemPrefabsArray.Length)].tag);
                        }
                        dots.Add(board.playingBoard[i, j]);
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = true;
                        if (dot.isAdjacentBomb)
                        {
                            dots.Remove(board.playingBoard[i, j]);
                            board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                            board.playingBoard[i, j].GetComponent<Gem>().isAdjacentBomb = false;
                            Transform transMarker = board.playingBoard[i, j].transform.Find("Adjacent Marker(Clone)");
                            transMarker.parent = null;
                            Destroy(transMarker.gameObject);

                            dots.Union(GetAdjacentPieces(i, j)).ToList();  
                        }
                    }  
                }
            }
        }
        return dots;
    }
    
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.playingBoard[column, i] != null)
            {
                Gem dot = board.playingBoard[column, i].GetComponent<Gem>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                if (dot.isAdjacentBomb)
                {
                    dots.Union(GetAdjacentPieces(column,i)).ToList();
                }
                if (dot.isColorBomb)
                {
                    MatchPiecesOfColor(board.gemPrefabsArray[Random.Range(0,board.gemPrefabsArray.Length)].tag );
                }
                dots.Add(board.playingBoard[column, i]);
                dot.isMatched = true;
            }
        }

        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.playingBoard[i, row] != null)
            {
                Gem dot = board.playingBoard[i, row].GetComponent<Gem>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                if (dot.isAdjacentBomb)
                {
                    dots.Union(GetAdjacentPieces(i, row)).ToList();
                }
                if (dot.isColorBomb)
                {
                    MatchPiecesOfColor(board.gemPrefabsArray[Random.Range(0, board.gemPrefabsArray.Length)].tag);
                }
                dots.Add(board.playingBoard[i, row]);
                dot.isMatched = true;
            }
        }

        return dots;
    }
    public void IsItARowOrColumn(MatchType matchType)
    {
        // did the player move something
        if (board.currentGem != null)
        {
            // is the piece they moved matched?
            if (board.currentGem.isMatched && board.currentGem.tag == matchType.color)
            {
                // make it unmatched so its stays on board
                board.currentGem.isMatched = false;
                
                if ((board.currentGem.SwipeAngle >-45 && board.currentGem.SwipeAngle <= 45)
                    || (board.currentGem.SwipeAngle <-135 || board.currentGem.SwipeAngle >= 135))
                {
                    board.currentGem.MakeRowBomb();
                    
                }
                else
                {
                    board.currentGem.MakeColumnBomb();
                }
            }
            
            // is other piece match matched?
            else if (board.currentGem.otherGem != null)
            {
                Gem otherDot = board.currentGem.otherGem.GetComponent<Gem>();
                //Is the other dot matches?
                if (otherDot.isMatched && otherDot.tag == matchType.color)
                {
                    // make it unmatched
                    otherDot.isMatched = false;
                    if ((board.currentGem.SwipeAngle > -45 && board.currentGem.SwipeAngle <= 45)
                    || (board.currentGem.SwipeAngle < -135 || board.currentGem.SwipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                       
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
            
           
        }
    }
 
   
   
}
