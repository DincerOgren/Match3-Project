using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Vector2Int gridValues;
    public GameObject backgroundPrefab;
    private Tile selectedTileA;
    private Tile selectedTileB;
    Vector2 backgroundSizeThreshold;

    [SerializeField] float _cycleDuration = 1f;

    public Tile[,] grid;
    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        grid = new Tile[gridValues.x, gridValues.y];

    }

    private void Start()
    {


        CalculateTileSize();
    }
    void CalculateTileSize()
    {
        // Get the size of the tile based on its SpriteRenderer bounds
        SpriteRenderer sr = backgroundPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Calculate the world size of the tile (accounting for its scale)
            backgroundSizeThreshold = sr.bounds.size;
        }

    }
    public void SelectTile(Tile tile)
    {
        if (selectedTileA == null)
        {
            // First tile selected
            print("Tile Selected1");
            selectedTileA = tile;
            tile.HighlightTile(true);
        }
        else if (selectedTileB == null)
        {
            // Second tile selected
            selectedTileB = tile;
            print("Tile Selected2");

            tile.HighlightTile(true);

            // Check if the two tiles are adjacent before swapping
            if (AreTilesAdjacent(selectedTileA, selectedTileB))
            {
                print("Should Swap");
                SwapTiles(selectedTileA, selectedTileB);
            }
            else
                print("ShouldntSwap");

            // Deselect tiles
            selectedTileA.HighlightTile(false);
            selectedTileB.HighlightTile(false);
            selectedTileA = null;
            selectedTileB = null;
        }
    }

    private bool AreTilesAdjacent(Tile tileA, Tile tileB)
    {
        // Check if tiles are adjacent (horizontal or vertical)

        print("x Mesafe = " + Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x) +
            " ,y mesafe" + Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y));
        return Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x) <= backgroundSizeThreshold.x && tileA.transform.position.y == tileB.transform.position.y ||
               Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y) <= backgroundSizeThreshold.y && tileA.transform.position.x == tileB.transform.position.x;
    }

    private void SwapTiles(Tile tileA, Tile tileB)
    {
        //// Swap positions
        //Vector3 tempPosition = tileA.transform.position;
        //tileA.transform.position = tileB.transform.position;
        //tileB.transform.position = tempPosition;

        Vector2 tileAStartPos = tileA.transform.position;
        Vector2 tileBStartPos = tileB.transform.position;

        tileA.transform.DOMove(tileBStartPos, _cycleDuration);
        tileB.transform.DOMove(tileAStartPos, _cycleDuration);


        //print("Before change "+GameManager.Instance.grid[tileA.index.x,tileA.index.y].objectType + "index = " + tileA.index);
        //print("Before change "+GameManager.Instance.grid[tileB.index.x,tileB.index.y].objectType + "index = " + tileB.index);
        ArraySwap(tileA, tileB);

        //print("After change " + GameManager.Instance.grid[tileA.index.x, tileA.index.y].objectType+ "index = "+tileA.index);
        //print("After change " + GameManager.Instance.grid[tileB.index.x, tileB.index.y].objectType + "index = " + tileB.index);

        // SHOULD SWAP ARRAY TOO +check

        //check if there is match 
        CheckForMatches();
        //else return start positions

    }

    private void ArraySwap(Tile tileA, Tile tileB)
    {
        Vector2Int tempTile;

        tempTile = tileA.index; //GameManager.Instance.grid[tileA.index.x,tileA.index.y];
        print("temp index = " + tempTile);
        tileA.index = tileB.index;
        tileB.index = tempTile;
        print("After change index = " + tileA.index);
        print("After change index = " + tileB.index);

        //still have some issues need to fix it

        grid[tileA.index.x, tileA.index.y] = tileB;
        grid[tileB.index.x, tileB.index.y] = tileA;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckForMatches();
        }
    }

    public void CheckForMatches()
    {
        for (int x = 0; x < gridValues.x; x++)
        {
            for (int y = 0; y < gridValues.y; y++)
            {
                // GRID IS SOMEHOW NULL SHOULD FIX THAT IMMIDEATLY
                print(grid[x, y].name);

                Tile currentTile = grid[x, y];
                //print("currenttile " + currentTile.name + " " + currentTile.objectType);
                if (currentTile != null)
                {
                    CheckHorizontalMatch(x, y);
                    CheckVerticalMatch(x, y);

                    
                }
                else
                {
                    print("NULLS");
                }
            }
        }

        DropTiles();
    }

    private void CheckHorizontalMatch(int startX, int startY)
    {
        Tile startTile = grid[startX, startY];

        int matchCount = 1;

        for (int x = startX + 1; x < gridValues.x; x++)
        {
            Tile nextTile = grid[x, startY];
            if (nextTile != null && nextTile.objectType == startTile.objectType)
            {
                matchCount++;
            }
            else
            {
                break;
            }
        }

        // If a match of 3 or more is found
        if (matchCount >= 3)
        {
            Debug.Log($"Horizontal match found starting at {startX}, {startY}!");
            for (int i = 0; i < matchCount; i++)
            {
                RemoveTile(grid[startX + i, startY]);
                grid[startX + i, startY].objectType = ObjectType.empty;
            }
        }
    }

    private void CheckVerticalMatch(int startX, int startY)
    {
        Tile startTile = grid[startX, startY];

        int matchCount = 1;

        for (int y = startY + 1; y < gridValues.y; y++)
        {
            Tile nextTile = grid[startX, y];
            if (nextTile != null && nextTile.objectType == startTile.objectType)
            {
                matchCount++;
                //print("Match found on tile[" + startX + "," +  "]");
            }
            else
            {
                break;
            }
        }

        // If a match of 3 or more is found
        if (matchCount >= 3)
        {
            Debug.Log($"Vertical match found starting at {startX}, {startY}!");
            for (int i = 0; i < matchCount; i++)
            {
                RemoveTile(grid[startX, startY + i]);
                grid[startX, startY + i].objectType = ObjectType.empty;
            }
        }

    }

    void DropTiles()
    {
        //Check if its block is null?

        for (int i = 0; i < gridValues.x; i++)
        {
            for (int y = gridValues.y - 1; y > 0; y--)
            {

            
                Tile downTile = grid[i, y];
                print("Vector(" + i + "," + y+")");
                if (downTile.objectType != ObjectType.empty)
                {
                    continue;
                }
                else
                {
                    int z = y;
                    while (true)
                    {
                        print("!!MOVING OBJECT" + " i= "+i+" , "+ " y = "+z);
                        grid[i,z+1].transform.DOMove(grid[i, z - 1].transform.position, _cycleDuration);

                        grid[i, z] = grid[i, z+1];
                        if (z >= gridValues.y-2)
                        {
                            break;
                        }
                        z++;
                    }

                    break;
                }
            }

        }
    }

    private void RemoveTile(Tile tile)
    {
        if (tile != null)
        {
            tile.gameObject.SetActive(false);
            // Destroy(tile.gameObject);
            //Adjust score, spawn new symbols etc.

        }
    }

    public void SetGrid(Vector2Int index, Tile tile)
    {
        grid[index.x, index.y] = tile;
    }
}
