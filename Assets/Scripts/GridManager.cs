using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 8, gridHeight = 8; // Set your desired grid dimensions
    public GameObject backgroundTile; // Drag your tile prefab here in the inspector
    public Transform gridParent; // Optional: Create an empty GameObject in the Hierarchy to parent all tiles

    private Vector2 tileSize; // Store tile size for spacing calculation
    public TileObject[] tiles;

    public GameObject tilePrefab;
    void Start()
    {
        // Get the size of the tile based on its sprite renderer's bounds
        SpriteRenderer tileRenderer = tilePrefab.GetComponent<SpriteRenderer>();
        if (tileRenderer != null)
        {
            tileSize = tileRenderer.bounds.size; // This gives the real world size of the tile
        }
        else
        {
            tileSize = new Vector2(1, 1); // Default to 1x1 if no SpriteRenderer is found
        }

        GenerateGrid();
    }

    void GenerateGrid()
    {
        Vector3 gridOffset = Vector3.zero;

        // If a grid parent is provided, use its position as an offset
        if (gridParent != null)
        {
            gridOffset = gridParent.position;
        }

        // Generate the grid relative to the parent's position
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calculate the new position based on the tile size and parent offset
                Vector2 position = new Vector2(x * tileSize.x, y * tileSize.y) + new Vector2(gridOffset.x, gridOffset.y);

                // Instantiate the tile at the calculated position
                GameObject newTile = Instantiate(tilePrefab, position, Quaternion.identity);

                BackgroundHolder bg = newTile.GetComponent<BackgroundHolder>();

                bg.tiles = tiles;
                bg.tilePrefab = tilePrefab;


                // Set the new tile under the grid parent in the Hierarchy
                if (gridParent != null)
                {
                    newTile.transform.SetParent(gridParent);
                }

                // You can optionally rename tiles for better readability in the hierarchy
                newTile.name = $"Tile_{x}_{y}";
            }
        }
    }
}
