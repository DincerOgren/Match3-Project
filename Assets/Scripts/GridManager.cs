using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public TileSlot tileSlotPrefab;
    public Tile tilePrefab;
    public int gridWidth = 5;
    public int gridHeigth = 5;
    public int sizeOffset = 20;
    public TileSlot[,] gridArray;

    public TileObject[] allTiles;

    public Vector2 tileSize;
    public Transform tileParent;
    public Transform gridParent;
    void Start()
    {
        gridArray = new TileSlot[gridWidth, gridHeigth];

        CalculateTileSize();

        GenerateGrid();
    }
    void CalculateTileSize()
    {
        // Get the size of the tile based on its SpriteRenderer bounds
        SpriteRenderer sr = tileSlotPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Calculate the world size of the tile (accounting for its scale)
            print("Calculated size");
            tileSize = sr.bounds.size;
        }
        else
        {
            // If no SpriteRenderer is present, use the local scale as an approximation
            tileSize = new Vector2(tileSlotPrefab.transform.localScale.x, tileSlotPrefab.transform.localScale.y);
        }
    }
    private void GenerateGrid()
    {
        Vector3 gridOffset = Vector3.zero;

        // If a grid parent is provided, use its position as an offset
        if (gridParent != null)
        {
            gridOffset = gridParent.position;
        }



        for (int y = 0; y < gridHeigth; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 position = new Vector2(x * tileSize.x, y * tileSize.y) + (Vector2)gridOffset;

                TileSlot temp = Instantiate(tileSlotPrefab, position, Quaternion.identity);
                temp.tileNum = new(x, y);


                //Spawn Tile
                TileObject randomObject = allTiles[Random.Range(0, allTiles.Length)];
                Tile newTile = Instantiate(tilePrefab, tileParent);
                newTile.transform.position = temp.transform.position;
                newTile.GetComponent<SpriteRenderer>().sprite = randomObject.sprite;
                newTile.GetComponent<SpriteRenderer>().color = randomObject.color;
                newTile.objectType = randomObject.objectType;
                newTile.index = temp.tileNum;
                newTile.SetSlot(temp);

                temp.AssignTile(newTile);
                
                temp.transform.parent = gridParent;
                temp.name = $"Tile_{x}_{y}";
                gridArray[x, y] = temp;
            }
        }

        CheckTiles();
    }

    private void CheckTiles()
    {
        for (int i = 0; i < gridHeigth; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                
               

            }
        }
    }
}
