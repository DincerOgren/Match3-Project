using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public TileSlot tileSlotPrefab;
    public Tile tilePrefab;
    Vector2Int _gridSize;
    public int sizeOffset = 20;
    public TileSlot[,] gridArray;

    public TileObject[] allTiles;

    public Vector2 tileSize;
    public Transform tileParent;
    public Transform gridParent;


    private void Awake()
    {
        Instance = this;



    }
    void Start()
    {

        GetGridSizeFromManager();

        gridArray = new TileSlot[_gridSize.x, _gridSize.y];

        CalculateTileSize();

        GenerateGrid();



    }

    private void GetGridSizeFromManager()
    {
        _gridSize = GameManager.Instance.GetGridSize();
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



        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                Vector2 position = new Vector2(x * tileSize.x, y * tileSize.y) + (Vector2)gridOffset;

                TileSlot temp = Instantiate(tileSlotPrefab, position, Quaternion.identity);
                temp.tileIndex = new(x, y);


                //Spawn Tile
                TileObject randomObject = allTiles[Random.Range(0, allTiles.Length)];
                Tile newTile = Instantiate(tilePrefab, tileParent);
                newTile.transform.position = temp.transform.position;
                newTile.GetComponent<SpriteRenderer>().sprite = randomObject.sprite;
                newTile.GetComponent<SpriteRenderer>().color = randomObject.color;
                newTile.objectType = randomObject.objectType;
                newTile.tilePoint = randomObject.tilePoints;
                // newTile.index = temp.tileNum;
                newTile.AssignSlot(temp);

                temp.SetTile(newTile);

                temp.transform.parent = gridParent;
                temp.name = $"Tile_{x}_{y}";
                gridArray[x, y] = temp;
            }
        }


    }

    public List<Tile> SpawnTile(int x, int amount)
    {
        List<Tile> extraTileList= new();
        for (int i = 1; i <= amount; i++)
        {
            TileObject randomObject = allTiles[Random.Range(0, allTiles.Length)];
            Tile newTile = Instantiate(tilePrefab, tileParent);
            newTile.transform.position = new Vector2(x * tileSize.x +gridParent.position.x, _gridSize.y + (tileSize.y*i));
            newTile.GetComponent<SpriteRenderer>().sprite = randomObject.sprite;
            newTile.GetComponent<SpriteRenderer>().color = randomObject.color;
            newTile.objectType = randomObject.objectType;
            newTile.tilePoint = randomObject.tilePoints;
            extraTileList.Add(newTile);
        }

        print("Spawned OBJECTS");
        return extraTileList;
    }


}
