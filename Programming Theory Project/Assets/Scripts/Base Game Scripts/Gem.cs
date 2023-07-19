using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private int column;
    private int row;
    // Using Properties for safer coding... EXAMPLE OF ENCAPSULATION !!!
    public int Column
    {
        get { return column; }
        set
        {
            if (value < 0)
            {
                Debug.LogError("You can't have a negative column");
            }
            else
            {
                column = value;
            }
        }
    }
    public int Row
    {
        get{ return row; }

        set
        {
                if (value < 0)
                {
                    Debug.LogError("You can't have a negative row");
                }
                else
                {
                    row = value;
                }
            
        }
    }
    
    private int previousColumn;
    private int previousRow;
    private int targetX;
    private int targetY;
    public bool isMatched = false;

    private Animator anim;
    private float shineDelay;
    private float shineDelaySeconds;
    private EndGameManager endGameManager;
    private HintManager hintManager;
    private MatchManager findMatches;
    private Board board;
    public GameObject otherGem;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    private float swipeAngle = 0f;
    public float SwipeAngle
    {
        get {return swipeAngle;}
        set {swipeAngle = value;}
    }
    [Header("Speed and Swipe Variables")]
    public float swipeResist = 1f;
    public float dotLerpingSpeed;
    public float CheckMoveDelay = .5f;

    [Header("Special Abilities")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public GameObject adjacentMarker;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    
    // Start is called before the first frame update
    void Start()
    {
        isColorBomb = false;
        isColumnBomb = false;
        isRowBomb = false;
        isAdjacentBomb = false;

        shineDelay = Random.Range(8f, 16f);
        shineDelaySeconds = shineDelay;
        anim = GetComponent<Animator>();
        endGameManager = FindObjectOfType<EndGameManager>();
        hintManager = FindObjectOfType<HintManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        findMatches = FindObjectOfType<MatchManager>();
       
    }
    // this is For testing and Debug Only
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
            // this.tag = "Color";
        }
    }
    // Update is called once per frame
    
    void Update()
    {
        
        targetX = Column;
        targetY = Row;

        // EXAMPLE OF ABSTRACTION!!!
        Shine();
           
        if (Mathf.Abs(targetX - transform.position.x) > 0)
        {
            CheckDistanceX();
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0)
        {
            CheckDistanceY();
        }
       
    }
    void Shine()
    {
        shineDelaySeconds -= Time.deltaTime;

        if (shineDelaySeconds <= 0)
        {
            shineDelaySeconds = shineDelay;
            StartCoroutine(StartShineCo());
        }
    }
    void  CheckDistanceX()
    {
        //checking difference of TargetX and its Transform.position.x
        //Move Towards the TargetX
        tempPosition = new Vector2(targetX, transform.position.y);
        transform.position = Vector2.Lerp(transform.position, tempPosition, dotLerpingSpeed);

        if (board.playingBoard[Column, Row] != this.gameObject)
        {
            board.playingBoard[Column, Row] = this.gameObject;
            
        }


        if (Mathf.Abs(targetX - transform.position.x) < .1)
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            findMatches.FindAllMatches();
        } 
    }
    void CheckDistanceY()
    {
        //checking difference of TargetY and its Transform.position.y
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the Target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, dotLerpingSpeed);
            if (board.playingBoard[column, row] != this.gameObject)
            {
                board.playingBoard[column, row] = this.gameObject; 
            }

        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            findMatches.FindAllMatches();
        }
    }
    IEnumerator StartShineCo()
    {
        anim.SetBool("Shine", true);
        yield return null;
        anim.SetBool("Shine", false);
    }
    public void PopAnimation()
    {
        anim.SetBool("Popped", true);
    }
    private void OnMouseDown()
    {
        
            //Destroy the hint
            if (hintManager != null)
            {
                hintManager.DestroyHint();
            }


            if (board.currentState == GameState.move)
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            }
        
    }
    private void OnMouseUp()
    {
        //if (ownerOfDot == OwnerOfDot.player1)
        //{
            if (board.currentState == GameState.move)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        //} 
    }
    void CalculateAngle()
    {
        if (Mathf.Abs( finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            GetTheDirectionFromAngle();
            board.currentGem = this;
        }
        else
        {
            board.currentState = GameState.move;
            
        }
        
    }
    void GetTheDirectionFromAngle()
    {
        if (swipeAngle >-45 && swipeAngle <= 45 && Column< board.width-1)
        {
            MoveTheGem(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && Row< board.height-1)
        {
            MoveTheGem(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && Column > 0)
        {
            MoveTheGem(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle > -135 && Row > 0)
        {
            MoveTheGem(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        } 
    }
    void MoveTheGem(Vector2 direction)
    {
        otherGem = board.playingBoard[Column +(int)direction.x, Row + (int)direction.y];
        previousRow = Row;
        previousColumn = Column;

        //checking if locktiles exist at location.
        if (board.lockTiles[Column, Row] == null && board.lockTiles[Column + (int)direction.x, Row + (int)direction.y] == null) 
        {
            if (otherGem != null)
            {
                otherGem.GetComponent<Gem>().Column += -1 * (int)direction.x;
                otherGem.GetComponent<Gem>().Row += -1 * (int)direction.y;
                Column += (int)direction.x;
                Row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = GameState.move;
            }
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    public IEnumerator CheckMoveCo()
    {
        CheckForSpecials();
        yield return new WaitForSeconds(.25f);
        // Validating if the move was legal by the rules.
        if (otherGem != null)
        {
            if (!isMatched && !otherGem.GetComponent<Gem>().isMatched)
            {
                otherGem.GetComponent<Gem>().row = row;
                otherGem.GetComponent<Gem>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(CheckMoveDelay);
                board.currentGem = null;
                board.currentState = GameState.move;
            }
            else
            {
                if (endGameManager != null)
                {
                    if (endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                StartCoroutine(board.DestroyMatchesCo());
                

            }
        }
        
    }
    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
        
    }
    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        } 
    }
    public void MakeColorBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";

        }
        
    }
    public void MakeAdjacentBomb()
    {
       
        if (!isRowBomb && !isColumnBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
        
    }
    public void CheckForSpecials()
    {
        // Checking if both pieces are color bombs
        if (isColorBomb && otherGem.GetComponent<Gem>().isColorBomb)
        {
            for (int i = 0; i < board.width; i++)
            {
                for (int j = 0; j < board.height; j++)
                {
                    if (board.playingBoard[i, j] != null)
                    {
                        Gem piece = board.playingBoard[i, j].GetComponent<Gem>();
                        piece.isMatched = true;
                    }

                }
            }
            otherGem.GetComponent<Gem>().isMatched = true;
        }
        // Checking if one piece is color and other isRow
        else if ((isColorBomb && otherGem.GetComponent<Gem>().isRowBomb) || (isRowBomb && otherGem.GetComponent<Gem>().isColorBomb))
        {
            if (this.tag == "Color")
            {
                findMatches.MakeOneColorRowColumnOrAdjacent(otherGem.tag, "Row");
                this.isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }
            else
            {
                findMatches.MakeOneColorRowColumnOrAdjacent(this.tag, "Row");
                this.isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }
            
        }
        // Checking if one piece is color and other isColumn
        else if ((isColorBomb && otherGem.GetComponent<Gem>().isColumnBomb) || (isColumnBomb && otherGem.GetComponent<Gem>().isColorBomb))
        {
            if (this.tag == "Color")
            {
                findMatches.MakeOneColorRowColumnOrAdjacent(otherGem.tag, "Column");
                this.isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }
            else
            {
                findMatches.MakeOneColorRowColumnOrAdjacent(this.tag, "Column");
                this.isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }
        }
        //Checking if One Piece is Color and other isAdjacent
        else if ((isColorBomb && otherGem.GetComponent<Gem>().isAdjacentBomb) || (isAdjacentBomb && otherGem.GetComponent<Gem>().isColorBomb))
        {
            if (this.tag == "Color")
            {
                findMatches.MakeOneColorRowColumnOrAdjacent(otherGem.tag, "Adjacent");
                this.isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }
            else
            {
                findMatches.MakeOneColorRowColumnOrAdjacent(this.tag, "Adjacent");
                this.isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }
        }
        // Checking if Piece is Color and other is Normal
        else if (isColorBomb)
        {
            // This piece is a color bomb, and the other
            findMatches.MatchPiecesOfColor(otherGem.tag);
            isMatched = true;
        }
        // Checking if other piece is Color and this is Normal
        else if (otherGem.GetComponent<Gem>().isColorBomb)
        {
            //The other piece is a color bomb and this piece has the color to destroy
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherGem.GetComponent<Gem>().isMatched = true;
        }
        // Checking if this piece isRow and other isRow or isColumn
        else if (isRowBomb && (otherGem.GetComponent<Gem>().isRowBomb || otherGem.GetComponent<Gem>().isColumnBomb))
        {
            GetRowAndColoumnSpecial(this.column, this.row);
        }
        // Checking if this piece isColumn and other isRow or isColumn
        else if (isColumnBomb && (otherGem.GetComponent<Gem>().isRowBomb || otherGem.GetComponent<Gem>().isColumnBomb))
        {
            GetRowAndColoumnSpecial(this.column, this.row);
        }
        // Checking if this piece isAdjacent and other isrow or iscolumn
        else if (isAdjacentBomb && (otherGem.GetComponent<Gem>().isRowBomb || otherGem.GetComponent<Gem>().isColumnBomb))
        {
            Get3RowsAnd3ColoumnSpecial(this.column,this.row);
        }
        // Checking if this piece isRow or isColumn and other isAdjacent
        else if ((isRowBomb || isColumnBomb) && otherGem.GetComponent<Gem>().isAdjacentBomb)
        {
            Get3RowsAnd3ColoumnSpecial(this.column, this.row);
        }
        else if (isAdjacentBomb && otherGem.GetComponent<Gem>().isAdjacentBomb)
        {
            GetMegaAdjacentPieces(this.column,this.row);
            this.isMatched = true;
            otherGem.GetComponent<Gem>().isMatched = true;
        }


    }
    void GetRowAndColoumnSpecial(int column,int row)
    {
        for (int i = 0; i < board.width; i++)
        {
            if (board.playingBoard[i,row] != null)
            {
                board.playingBoard[i, row].GetComponent<Gem>().isMatched = true;
            } 
        }
        for (int j = 0; j < board.height; j++)
        {
            if (board.playingBoard[column, j] != null)
            {
                board.playingBoard[column, j].GetComponent<Gem>().isMatched = true;
            }
        }
    }
    void Get3RowsAnd3ColoumnSpecial(int column, int row)
    {
        for (int i = column-1; i < column+2; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (i >= 0 && i < board.width)
                {
                    if (board.playingBoard[i, j] != null)
                    {
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = true;
                    }
                }
            }  
        }
        for (int i = 0; i < board.width; i++)
        {
            for (int j = row-1; j < row+2; j++)
            {
                if (j >= 0 && j < board.height)
                {
                    if (board.playingBoard[i, j] != null)
                    {
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = true;
                    }
                }
            }
        }
    }
    public void GetMegaAdjacentPieces(int column, int row)
    {
        
        for (int i = column - 2; i <= column + 2; i++)
        {
            for (int j = row - 2; j <= row + 2; j++)
            {
                //checked if piece is inside board
                if (i >= 0 && i < board.width  && j >= 0 && j < board.height)
                {
                    if (board.playingBoard[i, j] != null)
                    {
                        board.playingBoard[i, j].GetComponent<Gem>().isMatched = true; 
                    }
                }
            }
        }
       
    }

}
