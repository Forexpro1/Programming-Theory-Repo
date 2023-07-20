using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ATTENTION! SOME CREDIT GOES TO "MISTER TAFT CREATES" FOR  HIS TURORIAL ON 3D MATCH PUZZLES FOR THE INITIAL SETUP OF HOW TO CODE SUCH A GAME.
 * OUR CODES HAVE SIMLIAR WORKFLOWS, BUT I CHANGED MANY BEHAVIORS.
 * HE CAN BE FOUND ON "YOUTUBE" FOR REFERENCE.  
 * */

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum GemType
{
    Breakable,
    Blank,
    Lock,
    Concrete,
    Slime,
    Normal
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

[System.Serializable]
public class TileProperties
{
    public int x;
    public int y;
    public GemType gemType;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public World world;
    public int level;

    public GameState currentState = GameState.move;
    [Header("Board Dimensions")]
    public int width;
    public int height;
    public int yDistanceOffSet;
    

    [Header(" Prefabs")]
    public GameObject concreteTilePrefab;
    public GameObject SlimePiecePrefab;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject[] gemPrefabsArray;
    public GameObject destroyEffect;

    [Header("Layout")]
    public TileProperties[] boardLayout;
    private bool [,] blankSpaces;
    private SpecialTile[,] concreteTiles;
    private SpecialTile[,] breakableTiles;
    private SpecialTile[,] slimeTiles;
    [HideInInspector]
    public SpecialTile[,] lockTiles;
    public GameObject[,] playingBoard;

    [HideInInspector]
    public Gem currentGem;

    [Header("Gem Properties etc")]

    public int basePieceValue = 20;
    public int[] scoreGoals;

    [HideInInspector]
    public MatchType matchType;
    private int streakValue = 1;
    private int loopCount = 0;
    private bool makeSlime = true;
    private MatchManager matchManager;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    
    [Header("Speed Variables")]
    public float refillBoardDelay = .5f;
    public float particleDestoryEffectDelay = .5f;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    gemPrefabsArray = world.levels[level].gems;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        breakableTiles = new SpecialTile[width, height];
        lockTiles = new SpecialTile[width, height];
        concreteTiles = new SpecialTile[width, height];
        slimeTiles = new SpecialTile[width, height];
        matchManager = FindObjectOfType<MatchManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        blankSpaces = new bool[width, height];
        playingBoard = new GameObject[width, height];
        currentState = GameState.pause;
        Setup();
    }

    public void GenerateBlankSpaces()
    {
        
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].gemType == GemType.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    public void GenerateBreakableTiles()
    {
        // look for all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].gemType == GemType.Breakable)
            {
                // create a Jelly tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x,boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<SpecialTile>();
            }
        }
    }
    private void GenerateLockTiles()
    {
        // look for all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].gemType == GemType.Lock)
            {
                // create a Lock tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<SpecialTile>();
            }
        }

    }
    private void GenerateConcreteTiles()
    {
        // look for all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].gemType == GemType.Concrete)
            {
                // create a concrete tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<SpecialTile>();

                // create a background tile as well because it does not create one during Setup()
                Vector2 tilePosition = new Vector2(boardLayout[i].x, boardLayout[i].y);

                GameObject boardTile1 = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;

                boardTile1.transform.parent = this.transform;
            }
        }
    }
    private void GenerateSlimeTiles()
    {
        // look for all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].gemType == GemType.Slime)
            {
                // create a slime tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(SlimePiecePrefab, tempPosition, Quaternion.identity);
                 slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<SpecialTile>();

                // create a background tile as well because it does not create one during Setup()
                Vector2 tilePosition = new Vector2(boardLayout[i].x, boardLayout[i].y);

                GameObject boardTile1 = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;

                boardTile1.transform.parent = this.transform;
            }
        }
    }

    private void GenerateBackgroundTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {

                    Vector2 tempPositionP1 = new Vector2(i, j + yDistanceOffSet);
                    Vector2 tilePositionP1 = new Vector2(i, j);


                    GameObject backgroundTileP1 = Instantiate(tilePrefab, tilePositionP1, Quaternion.identity) as GameObject;

                    backgroundTileP1.transform.parent = this.transform;

                    backgroundTileP1.name = "( " + i + "," + j + " )";

                    int randomGem = Random.Range(0, gemPrefabsArray.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, gemPrefabsArray[randomGem]) && maxIterations < 100)
                    {
                        randomGem = Random.Range(0, gemPrefabsArray.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    GameObject gem = Instantiate(gemPrefabsArray[randomGem], tempPositionP1, Quaternion.identity);
                    gem.GetComponent<Gem>().Row = j;
                    gem.GetComponent<Gem>().Column = i;
                    gem.transform.parent = this.transform;

                    playingBoard[i, j] = gem;
                }
            }

        }
    }
    // EXAMPLE OF ABSTRACTION
    private void Setup()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        GenerateBackgroundTiles();
    }
    public  IEnumerator DestroyMatchesCo()
    {
        yield return new WaitForSeconds(.25f);
        
        if (matchManager.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        matchManager.currentMatches.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBoard[i,j] != null)
                {
                    DestroyMatchesAt(i, j); // If bool "isMatched" in Gem.cs is true. it will destroys gameobject and nullify in the playingBoardarray.
                }
            }
        }
        
        StartCoroutine(DecreaseRowCo());
    }
    private void CheckToMakeBombs()
    {
        //How many objects are in matchManager currentMatches?
        if (matchManager.currentMatches.Count > 3)
        {
            // What type of match?
            MatchType typeOfMatch = GetMatchType();

            if (typeOfMatch.type == 1)
            {
                //Make a color bomb
                // is the Current dot matched?
                if (currentGem != null && currentGem.isMatched && currentGem.tag == typeOfMatch.color)
                {
                    currentGem.isMatched = false;
                    currentGem.MakeColorBomb();
                }
                else
                {
                    if (currentGem.otherGem != null)
                    {
                        Gem otherGem = currentGem.otherGem.GetComponent<Gem>();
                        if (otherGem.isMatched && otherGem.tag == typeOfMatch.color)
                        {
                            otherGem.isMatched = false;
                            otherGem.MakeColorBomb();
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                // Make an Adjacent bomb
                // is the Current dot matched?
                if (currentGem != null && currentGem.isMatched && currentGem.tag == typeOfMatch.color)
                {
                    currentGem.isMatched = false;
                    currentGem.MakeAdjacentBomb();
                }
                else if (currentGem.otherGem != null)
                {
                    Gem otherGem = currentGem.otherGem.GetComponent<Gem>();
                    if (otherGem.isMatched && otherGem.tag == typeOfMatch.color)
                    {
                        otherGem.isMatched = false;
                        otherGem.MakeAdjacentBomb();
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                matchManager.IsItARowOrColumn(typeOfMatch);
            } 
            
            currentGem = null;
            
        }    
    }
    private MatchType GetMatchType()
    {
        
        matchType.type = 0;
        matchType.color = "";

        //Cycle through all of match Copy and decide if a bomb needs to be made
        for (int i = 0; i < matchManager.currentMatches.Count; i++)
        {
            if (matchManager.currentMatches[i] != null)
            {
                //Store this dot
                Gem thisGem = matchManager.currentMatches[i].GetComponent<Gem>();
                string color = matchManager.currentMatches[i].tag;
                int column = thisGem.Column;
                int row = thisGem.Row;
                int columnMatch = 0;
                int rowMatch = 0;
                // Cycle through the rest of the pieces and compare
                for (int j = 0; j < matchManager.currentMatches.Count; j++)
                {
                    // Store the next dot
                    Gem nextGem = matchManager.currentMatches[j].GetComponent<Gem>();

                    if (nextGem == thisGem)
                    {
                        continue;
                    }
                    if (nextGem.Column == thisGem.Column && nextGem.tag == color)
                    {
                        columnMatch++;
                    }
                    if (nextGem.Row == thisGem.Row && nextGem.tag == color)
                    {
                        rowMatch++;
                    }
                }
                // Return 3 if column or row match
                // Return 2 if adjacent
                // Return 1 if color bomb
                if (columnMatch >= 4 || rowMatch >= 4)
                {
                    if (currentGem == null)
                    {
                        currentGem = matchManager.currentMatches[i + 2].GetComponent<Gem>();
                    }
                    matchType.type = 1;
                    matchType.color = color;
                    return matchType;
                }
                else if ((columnMatch == 2 || columnMatch == 3) && rowMatch == 2)
                {
                    if (currentGem == null)
                    {
                        currentGem = matchManager.currentMatches[i].GetComponent<Gem>();
                    }
                    matchType.type = 2;
                    matchType.color = color;
                    return matchType;
                }
                else if (columnMatch == 2 && (rowMatch == 2 || rowMatch == 3))
                {
                    if (currentGem == null)
                    {
                        currentGem = matchManager.currentMatches[i].GetComponent<Gem>();
                    }
                    matchType.type = 2;
                    matchType.color = color;
                    return matchType;
                }
                else if (columnMatch == 3 || rowMatch == 3)
                {
                    if (currentGem == null)
                    {
                        currentGem = matchManager.currentMatches[i].GetComponent<Gem>();
                    }
                    matchType.type = 3;
                    matchType.color = color;
                    return matchType;
                }
            }   
        }
        matchType.type = 0;
        matchType.color = "";
        return matchType;

    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (playingBoard[column,row].GetComponent<Gem>().isMatched)
        {
            // Does a Tile need to break?
            if (breakableTiles[column,row] != null)
            {
                //If it does, give one damage
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column,row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

            // Does a lock tile need to break?
            if (lockTiles[column, row] != null)
            {
                //If it does, give one damage
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }
            // Destroy Concrete and Slime at adjacent locations if it exist
            DamageConcrete(column, row);
            DamageSlime(column,row);

            // Updating The Goal Manager.
            if (goalManager != null)
            {
                goalManager.CompareGoal(playingBoard[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            // Does the sound manager exist?
            if (soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();   
            }
            //Creating a Particle effect
            GameObject particle = Instantiate(destroyEffect, playingBoard[column,row].transform.position, Quaternion.identity);
            Destroy(particle, particleDestoryEffectDelay);

            // Destroying Gem at location and updating Score.
            Destroy(playingBoard[column, row], .5f);
            playingBoard[column, row].GetComponent<Gem>().PopAnimation();
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            playingBoard[column, row] = null;
        }
    }
    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {

            if (concreteTiles[column-1,row])
            {
                concreteTiles[column-1, row].TakeDamage(1);
                if (concreteTiles[column-1, row].hitPoints <= 0)
                {
                    concreteTiles[column-1, row] = null;
                }
            }
        }
        if (column < width-1)
        {
            if (concreteTiles[column +1, row])
            {
                concreteTiles[column +1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row-1])
            {
                concreteTiles[column, row-1].TakeDamage(1);
                if (concreteTiles[column, row-1].hitPoints <= 0)
                {
                    concreteTiles[column, row-1] = null;
                }
            }
        }
        if (row < height -1)
        {
            if (concreteTiles[column, row+1])
            {
                concreteTiles[column, row+1].TakeDamage(1);
                if (concreteTiles[column, row+1].hitPoints <= 0)
                {
                    concreteTiles[column, row+1] = null;
                }
            }
        }
    }
    private void DamageSlime(int column, int row)
    {
        if (column > 0)
        {

            if (slimeTiles[column - 1, row])
            {
                slimeTiles[column - 1, row].TakeDamage(1);
                if (slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    slimeTiles[column - 1, row] = null;
                    
                }
                makeSlime = false;
            }
            
        }
        if (column < width - 1)
        {
            if (slimeTiles[column + 1, row])
            {
                slimeTiles[column + 1, row].TakeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
            
        }
        if (row > 0)
        {
            if (slimeTiles[column, row - 1])
            {
                slimeTiles[column, row - 1].TakeDamage(1);
                if (slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
            
        }
        if (row < height - 1)
        {
            if (slimeTiles[column, row + 1])
            {
                slimeTiles[column, row + 1].TakeDamage(1);
                if (slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    slimeTiles[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
        
    }
    private IEnumerator DecreaseRowCo()
    {
        /* Moving every Gem down as long as each coordinate is not a blankSpace,
        the dot at location is empty(destroyed & null),not a concrete Tile,
        and not a Slime Tile. */
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i,j] && playingBoard[i,j] == null && !concreteTiles[i,j] && !slimeTiles[i,j])
                {
                    for (int k = j+1; k < height; k++)
                    {
                        // if a dot is found...
                        if (playingBoard[i,k]!= null)
                        {
                            // move that dot to this empty space
                            playingBoard[i, k].GetComponent<Gem>().Row = j;
                            // set that spot above to be null
                            playingBoard[i, k] = null;
                            // break out of the loop;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator FillBoardCo()
    {
        
        //yield return new WaitForSeconds(refillBoardDelay);
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard())
        {
            streakValue ++;
            StartCoroutine( DestroyMatchesCo());
            loopCount++;
            yield break;
        }
        currentGem = null;
        
        yield return new WaitForSeconds(.5f);
        loopCount = 0;
        
        CheckToMakeSlime();
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoardCo());
            Debug.Log("DeadLocked!!!");
        }
        if (currentState != GameState.pause)
        {
            currentState = GameState.move;
        } 
        makeSlime = true;
        streakValue = 1;
    }
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBoard[i, j] == null && !blankSpaces[i,j] && !concreteTiles[i,j] && !slimeTiles[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + yDistanceOffSet);
                    int dotToUse = Random.Range(0, gemPrefabsArray.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i,j,gemPrefabsArray[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, gemPrefabsArray.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(gemPrefabsArray[dotToUse], tempPosition, Quaternion.identity);
                    playingBoard[i, j] = piece;
                    piece.GetComponent<Gem>().Row = j;
                    piece.GetComponent<Gem>().Column = i;
                }
            }
        }
    }
    private bool MatchesAt( int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (playingBoard[column - 1, row] != null && playingBoard[column - 2, row]!= null)
            {
                if (playingBoard[column - 1, row].tag == piece.tag && playingBoard[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (playingBoard[column, row-1] != null && playingBoard[column,row-2] != null)
            {
                if (playingBoard[column, row - 1].tag == piece.tag && playingBoard[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }  
        }
        else if (column <= 1 || row <=1)
        {
            if (row >1)
            {
               if (playingBoard[column,row -1] != null && playingBoard[column,row-2] != null)
               {
                    if (playingBoard[column, row -1].tag == piece.tag && playingBoard[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
               } 
            }
            if (column > 1)
            {
                if (playingBoard[column- 1, row ] != null && playingBoard[column- 2, row ] != null)
                {
                    if (playingBoard[column - 1, row].tag == piece.tag && playingBoard[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }      
            }
        }
        return false;
    }
    private bool MatchesOnBoard()
    {

        matchManager.FindAllMatches();
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBoard[i,j] != null)
                {
                    if (playingBoard[i,j].GetComponent<Gem>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void CheckToMakeSlime()
    {
        // Check the slime Tile Array
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (slimeTiles[i,j] != null && makeSlime)
                {
                    // call another method to make a new slime
                    MakeNewSlime();
                    return;
                }
            }

        }
    }
    private void MakeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        while (!slime && loops < 200)
        {
            int randomX = Random.Range(0, width);
            int randomY = Random.Range(0,height);
            if (slimeTiles[randomX,randomY])
            {
                Vector2 adjacent = CheckForAdjacents(randomX,randomY);
                if (adjacent != Vector2.zero)
                {
                    Destroy(playingBoard[randomX + (int) adjacent.x, randomY + (int) adjacent.y]);
                    Vector2 tempPosition = new Vector2(randomX + (int)adjacent.x, randomY + (int)adjacent.y);
                    GameObject tile = Instantiate(SlimePiecePrefab, tempPosition, Quaternion.identity);
                    slimeTiles[randomX + (int)adjacent.x, randomY + (int)adjacent.y] = tile.GetComponent<SpecialTile>();
                    slime = true;
                }
            }   
            loops++;
        }
    }
    private Vector2 CheckForAdjacents(int column, int row)
    {
        if ( column < width-1 && playingBoard[column + 1, row] )
        {
            return Vector2.right;
        }
        if (column > 0 && playingBoard[column - 1, row])
        {
            return Vector2.left;
        }
        if (row < height - 1 && playingBoard[column, row +1])
        {
            return Vector2.up;
        }
        if (row > 0 && playingBoard[column, row-1])
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }
    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBoard[i,j] != null)
                {
                    if (i < width-1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        } 
                    }
                    if (j < height -1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (playingBoard[column+ (int) direction.x, row+ (int) direction.y] != null)
        {
            // Take second piece and save it in a holder
            GameObject holder = (GameObject)playingBoard[column + (int)direction.x, row + (int)direction.y];
            //Switching the first dot to be the second position
            playingBoard[column + (int)direction.x, row + (int)direction.y] = playingBoard[column, row];
            // Set the first dot to be the second dot
            playingBoard[column, row] = holder;

        }
        
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBoard[i,j] != null)
                {
                    // Make  sure that one and two to the right are in the board
                    if (i < width-2)
                    {
                        // check if the gemPrefabsArray to the right and two to the right exist
                        if (playingBoard[i+1, j] != null && playingBoard[i+2,j] != null)
                        {
                            if (playingBoard[i+1,j].tag == playingBoard[i,j].tag
                                && playingBoard[i+2, j].tag == playingBoard[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        // check if the gemPrefabsArray above exist
                        if (playingBoard[i,j+ 1] != null && playingBoard[i,j+ 2] != null)
                        {
                            if (playingBoard[i,j+ 1].tag == playingBoard[i,j].tag
                                && playingBoard[i,j+ 2].tag == playingBoard[i,j].tag)
                            {
                                return true;
                            }
                        }
                        
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator ShuffleBoardCo()
    {
        yield return new WaitForSeconds(refillBoardDelay);
        // Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        // Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBoard[i,j] != null)
                {
                    newBoard.Add(playingBoard[i, j]);
                }
            }
        }
        yield return new WaitForSeconds(refillBoardDelay);
        //for every spot on the board...
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // if this spot shouldn't be blank or concrete tiles
                if (!blankSpaces[i,j] && !concreteTiles[i,j] && !slimeTiles[i,j])
                {
                    // pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    
                    //Assign the colum and row to the piece
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    // Make a container for the piece
                    Gem piece = newBoard[pieceToUse].GetComponent<Gem>();
                    maxIterations = 0;
                    piece.Column = i;
                    piece.Row = j;
                    // fill in the gemPrefabsArray array with this new piece
                    playingBoard[i, j] = newBoard[pieceToUse];
                    // remove it form the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }

            }
        }
        // Check if it's still deadlocked
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoardCo());
        }
    }

    // These method used in MatchManager
    public void BombRowAffectingSpecialTiles(int row)
    {
        for (int i = 0; i < width; i++)
        {

            if (concreteTiles[i, row])
            {
                concreteTiles[i, row].TakeDamage(1);
                if (concreteTiles[i, row].hitPoints <= 0)
                {
                    concreteTiles[i, row] = null;
                }
            }
            if (slimeTiles[i, row])
            {
                slimeTiles[i, row].TakeDamage(1);
                if (slimeTiles[i, row].hitPoints <= 0)
                {
                    slimeTiles[i, row] = null;
                }
            }
        }
    }
    public void BombColumnAffectingSpecialTiles(int column)
    {

        for (int j = 0; j < height; j++)
        {
            if (concreteTiles[column, j])
            {
                concreteTiles[column, j].TakeDamage(1);
                if (concreteTiles[column, j].hitPoints <= 0)
                {
                    concreteTiles[column, j] = null;
                }
            }
            if (slimeTiles[column, j])
            {
                slimeTiles[column, j].TakeDamage(1);
                if (slimeTiles[column, j].hitPoints <= 0)
                {
                    slimeTiles[column, j] = null;
                }
            }

        }

    }
}
