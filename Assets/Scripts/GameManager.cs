using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Tile selectedTileA;
    private Tile selectedTileB;

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
        return Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x) == 1 && tileA.transform.position.y == tileB.transform.position.y ||
               Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y) == 1 && tileA.transform.position.x == tileB.transform.position.x;
    }

    private void SwapTiles(Tile tileA, Tile tileB)
    {
        // Swap positions
        Vector3 tempPosition = tileA.transform.position;
        tileA.transform.position = tileB.transform.position;
        tileB.transform.position = tempPosition;
    }
}
