using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject backgroundPrefab; // Drag your tile prefab here in the inspector
    public Transform gridParent; // Optional: Create an empty GameObject in the Hierarchy to parent all tiles

    private Vector2 tileSize; // Store tile size for spacing calculation
    public TileObject[] tiles;

    public GameObject tilePrefab;
    private Vector2Int gridValues; // Set your desired grid dimensions

    [SerializeField] Transform _objectParent;

    void Start()
    {

        SetGridValues();

        CalculateTileSize();
        GenerateGrid();
    }

    private void SetGridValues()
    {
        gridValues = GameManager.Instance.gridValues;
    }

    void CalculateTileSize()
    {
        // Get the size of the tile based on its SpriteRenderer bounds
        SpriteRenderer sr = backgroundPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Calculate the world size of the tile (accounting for its scale)
            tileSize = sr.bounds.size;
        }
        else
        {
            // If no SpriteRenderer is present, use the local scale as an approximation
            tileSize = new Vector2(backgroundPrefab.transform.localScale.x, backgroundPrefab.transform.localScale.y);
        }
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
        for (int x = 0; x < gridValues.x; x++)
        {
            for (int y = 0; y < gridValues.y; y++)
            {


                // Calculate the new position based on the tile size and parent offset
                Vector2 position = new Vector2(x * tileSize.x, y * tileSize.y) + (Vector2)gridOffset;


                // Instantiate the tile at the calculated position
                GameObject newTile = Instantiate(backgroundPrefab, position, Quaternion.identity);


                BackgroundHolder bg = newTile.GetComponent<BackgroundHolder>();
                bg.arrayIndex = new Vector2Int(x, y);
                bg.tiles = tiles;
                bg.tilePrefab = tilePrefab;
                bg.objectParent = _objectParent;


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
