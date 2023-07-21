using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This is the ConerStone Script for all Matching to take place by adding all common tags of 3 or more in a list.

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
    private List<GameObject> IsAdjacentBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (gem1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(gem1.Column, gem1.Row));
        }
        if (gem2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(gem2.Column, gem2.Row));
        }
        if (gem3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(gem3.Column, gem3.Row));
        }
        return currentDots;
    }
    private List<GameObject> IsRowBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (gem1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(gem1.Row));
            board.BombRowAffectingSpecialTiles(gem1.Row);
        }
        if (gem2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(gem2.Row));
            board.BombRowAffectingSpecialTiles(gem2.Row);
        }
        if (gem3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(gem3.Row));
            board.BombRowAffectingSpecialTiles(gem3.Row);
        }
        return currentDots;
    }
    private List<GameObject> IsColumnBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (gem1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(gem1.Column));
            board.BombColumnAffectingSpecialTiles(gem1.Column);
        }
        if (gem2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(gem2.Column));
            board.BombColumnAffectingSpecialTiles(gem2.Column);
        }
        if (gem3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(gem3.Column));
            board.BombColumnAffectingSpecialTiles(gem3.Column);
        }
        return currentDots;
    }
    private void AddToListAndMatch(GameObject gem)
    {
        if (!currentMatches.Contains(gem))
        {
            currentMatches.Add(gem);
        }
        gem.GetComponent<Gem>().isMatched = true;
    }
    private void GetNearbyPieces(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        AddToListAndMatch(gem1);
        AddToListAndMatch(gem2);
        AddToListAndMatch(gem3);
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

                    Gem currentGameObjectGem = currentGameObject.GetComponent<Gem>();
                    if (i > 0 && i < board.width-1)
                    {
                        GameObject leftGameObject = board.playingBoard[i - 1, j];
                        GameObject rightGameObject = board.playingBoard[i + 1, j];

                        if (leftGameObject != null && rightGameObject != null)
                        {
                            Gem rightGameObjectGem = rightGameObject.GetComponent<Gem>();
                            Gem leftGameObjectGem = leftGameObject.GetComponent<Gem>();
                            
                            if (leftGameObject.tag == currentGameObject.tag && rightGameObject.tag == currentGameObject.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftGameObjectGem, currentGameObjectGem, rightGameObjectGem));
                                currentMatches.Union(IsColumnBomb(leftGameObjectGem, currentGameObjectGem, rightGameObjectGem));
                                currentMatches.Union(IsAdjacentBomb(leftGameObjectGem, currentGameObjectGem, rightGameObjectGem));
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
                            Gem downGem = downwardGameObject.GetComponent<Gem>();
                            Gem upGem = upwardGameObject.GetComponent<Gem>();

                            if (upwardGameObject.tag == currentGameObject.tag && downwardGameObject.tag == currentGameObject.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upGem, currentGameObjectGem, downGem));
                                currentMatches.Union(IsRowBomb(upGem, currentGameObjectGem, downGem));
                                currentMatches.Union(IsAdjacentBomb(upGem, currentGameObjectGem, downGem));
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
                        //set that gem to be matched
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
                        //set that gem to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                        board.playingBoard[i, j].GetComponent<Gem>().MakeRowBomb();
                    }
                    if (board.playingBoard[i, j].tag == color && type == "Column")
                    {
                        //set that gem to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                        board.playingBoard[i, j].GetComponent<Gem>().MakeColumnBomb();
                    }
                    if (board.playingBoard[i, j].tag == color && type == "Adjacent")
                    {
                        //set that gem to be matched
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                        board.playingBoard[i, j].GetComponent<Gem>().MakeAdjacentBomb();
                    }
                }
            }
        }
    }
    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = column - 1; i <= column+1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //checked if piece is inside board
                if (i >= 0 && i < board.width && j >= 0 && j< board.height)
                {
                    if (board.playingBoard[i,j] != null)
                    {
                        Gem gem = board.playingBoard[i, j].GetComponent<Gem>();
                        if (gem.isRowBomb)
                        {
                            gems.Union(GetRowPieces(i)).ToList();
                        }
                        if (gem.isColorBomb)
                        {
                            MatchPiecesOfColor(board.gemPrefabsArray[Random.Range(0, board.gemPrefabsArray.Length)].tag);
                        }
                        gems.Add(board.playingBoard[i, j]);
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = true;
                        if (gem.isAdjacentBomb)
                        {
                            gems.Remove(board.playingBoard[i, j]);
                            board.playingBoard[i, j].GetComponent<Gem>().isMatched = false;
                            board.playingBoard[i, j].GetComponent<Gem>().isAdjacentBomb = false;
                            Transform transMarker = board.playingBoard[i, j].transform.Find("Adjacent Marker(Clone)");
                            transMarker.parent = null;
                            Destroy(transMarker.gameObject);

                            gems.Union(GetAdjacentPieces(i, j)).ToList();  
                        }
                    }  
                }
            }
        }
        return gems;
    }
    
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.playingBoard[column, i] != null)
            {
                Gem gem = board.playingBoard[column, i].GetComponent<Gem>();
                if (gem.isRowBomb)
                {
                    gems.Union(GetRowPieces(i)).ToList();
                }
                if (gem.isAdjacentBomb)
                {
                    gems.Union(GetAdjacentPieces(column,i)).ToList();
                }
                if (gem.isColorBomb)
                {
                    MatchPiecesOfColor(board.gemPrefabsArray[Random.Range(0,board.gemPrefabsArray.Length)].tag );
                }
                gems.Add(board.playingBoard[column, i]);
                gem.isMatched = true;
            }
        }

        return gems;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.playingBoard[i, row] != null)
            {
                Gem gem = board.playingBoard[i, row].GetComponent<Gem>();
                if (gem.isColumnBomb)
                {
                    gems.Union(GetColumnPieces(i)).ToList();
                }
                if (gem.isAdjacentBomb)
                {
                    gems.Union(GetAdjacentPieces(i, row)).ToList();
                }
                if (gem.isColorBomb)
                {
                    MatchPiecesOfColor(board.gemPrefabsArray[Random.Range(0, board.gemPrefabsArray.Length)].tag);
                }
                gems.Add(board.playingBoard[i, row]);
                gem.isMatched = true;
            }
        }

        return gems;
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
                Gem otherGem = board.currentGem.otherGem.GetComponent<Gem>();
                //Is the other gem matched?
                if (otherGem.isMatched && otherGem.tag == matchType.color)
                {
                    // make it unmatched
                    otherGem.isMatched = false;
                    if ((board.currentGem.SwipeAngle > -45 && board.currentGem.SwipeAngle <= 45)
                    || (board.currentGem.SwipeAngle < -135 || board.currentGem.SwipeAngle >= 135))
                    {
                        otherGem.MakeRowBomb();
                       
                    }
                    else
                    {
                        otherGem.MakeColumnBomb();
                    }
                }
            }
            
           
        }
    }
 
   
   
}
