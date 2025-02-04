using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] float _cycleLength = 2;
    [SerializeField] Vector2Int _gridLength = new(5, 5);

    public Tile tileA;
    public Tile tileB;


    TileSlot[,] grid;

    public List<int> matchQuantity;

    public List<TileSlot> matchList;
    public List<TileSlot> tempMatchList;
    public List<TileSlot> tempLList;
    public List<TileSlot> lMatchesList;

    private void Awake()
    {
        Instance = this;
    }
    private IEnumerator Start()
    {
        yield return null;
        if (GridManager.Instance == null)
        {
            Debug.LogError("GridManager.Instance is NULL!");
        }
        else if (GridManager.Instance.gridArray == null)
        {
            Debug.LogError("gridArray is NULL in GridManager!");
        }


        grid = GridManager.Instance.gridArray;
    }


    public void SelectTile(Tile tile)
    {
        if (tileA == null)
        {
            tileA = tile;
            tileA.HighlightTile(true);
        }
        else if (tileB == null)
        {
            tileB = tile;
            tileB.HighlightTile(true);
            if (AreTilesAdjacent(tileA, tileB))
            {
                print("Should Swap");
                SwapTiles(tileA, tileB);
            }
            else
                print("Shouldnt Swap");



            tileA.HighlightTile(false);
            tileB.HighlightTile(false);
            tileA = null;
            tileB = null;
        }


    }

    private void SwapTiles(Tile tileA, Tile tileB)
    {
        // Check if they are same objects
        //inside swap
        //if (tileA.objectType == tileB.objectType)
        {
            Vector2 aPos = tileA.GetSlot().transform.position;

            var aSlot = tileA.GetSlot();
            var bSlot = tileB.GetSlot();



            // get temp pos = a
            // a = b
            ////b = temp
            //tileA.transform.DOMove(tileB.GetSlot().transform.position, _cycleLength).SetEase(Ease.InOutSine);
            //tileB.transform.DOMove(aPos, _cycleLength).SetEase(Ease.InOutSine);

            aSlot.SetTile(tileB);
            bSlot.SetTile(tileA);

        }
        //else
        {
            // do nothing
            // swap but return back object;

        }
    }

    private bool AreTilesAdjacent(Tile tileA, Tile tileB)
    {
        Vector2Int result = tileA.index - tileB.index;

        Vector2Int absResult = new(Mathf.Abs(result.x), Mathf.Abs(result.y));

        if (absResult.x > 1 || absResult.y > 1)
        {
            return false;
        }
        else if (absResult.x == absResult.y)
        {
            return false;
        }
        else
            return true;


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Try matches");



            Try();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SearchLShape();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            matchList.Clear();
            lMatchesList.Clear();
        }
    }
    private void CheckMatches()
    {

        // Horizontal Match
        //int rightMatchCounter = 0;


        for (int y = 0; y < 1; y++)
        {

            for (int x = 0; x < _gridLength.x; x++)
            {
                var firstTile = grid[x, y];
                if (x + 1 >= _gridLength.x)
                {
                    print("continue");
                    continue;
                }


                // GetNextTileOnGrid(firstTile.tileNum.x, firstTile.tileNum.y, 0);


            }
        }
    }

    void Try()
    {
        int row = 0;
        //horizontal
        TileSlot firstTile;

        int rightLink = 0;
        //length -2 ?
        for (int y = 0; y < _gridLength.y; y++)
        {
            print("in most up for :" + y);
            row = 0;

            while (row <= _gridLength.x - 2)
            {
                print("in while row : " + row);

                firstTile = grid[row, y];

                if (matchList.Contains(firstTile))
                {
                    row++;
                    continue;
                }

                tempMatchList.Add(firstTile);

                for (int i = row + 1; i < _gridLength.x; i++)
                {
                    print("in for : " + i);
                    if (firstTile.GetTile().objectType == grid[i, y].GetTile().objectType)
                    {
                        tempMatchList.Add(grid[i, y]);
                        rightLink++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (rightLink >= 2)
                {
                    print("match found after for");
                    matchList.AddRange(tempMatchList);

                    matchQuantity.Add(rightLink + 1);

                }

                rightLink = 0;
                tempMatchList.Clear();

                row++;
            }

        }


    }

    void SearchLShape()
    {
        if (matchList.Count == 0)
        {
            print("Matchlist empty");
            return;
        }

        int k = 0;
        int upLinks = 0;
        int downLinks = 0;
        // Cycle through every tile and find up and down links
        while (k < matchList.Count)
        {
            upLinks = 0;
            downLinks = 0;
            //find up links 
            TileSlot currentSlot = matchList[k];
            for (int i = currentSlot.tileIndex.y + 1; i <= _gridLength.y - currentSlot.tileIndex.y; i++)
            {
                if (grid[currentSlot.tileIndex.x, i].GetTile().objectType == currentSlot.GetTile().objectType)
                {
                    upLinks++;
                    tempLList.Add(grid[currentSlot.tileIndex.x, i]);
                    print(currentSlot.name + " icin " + "uplink bulundu adi : " + grid[currentSlot.tileIndex.x, i].name);
                }
                else
                    break;
            }

            //find down links

            for (int i = currentSlot.tileIndex.y - 1; i >= 0; i--)
            {
                if (grid[currentSlot.tileIndex.x, i].GetTile().objectType == currentSlot.GetTile().objectType)
                {
                    downLinks++;
                    tempLList.Add(grid[currentSlot.tileIndex.x, i]);
                    print(currentSlot.name + " icin " + "downlink bulundu adi : " + grid[currentSlot.tileIndex.x, i].name);

                }
                else
                    break;
            }

            if (upLinks + downLinks >= 2)
            {
                print("We find a L shape match");
                lMatchesList = new(tempLList);
                //Remove from verticalList?
            }

            tempLList.Clear();

            k++;

        }
    }

    void GetNextTileOnGrid(int x, int y, int rightLinkCount = 0)
    {
        int rightLink = rightLinkCount;
        TileSlot firstTile = grid[x, y];

        if (firstTile == null)
        {
            print("First tile empty");
            return;
        }


        if (x + 1 > _gridLength.x)
        {
            print("X lenghten büyüktrür");
            return;
        }

        if (grid[x + 1, y].GetTile().objectType == firstTile.GetTile().objectType)
        {
            tempMatchList.Add(firstTile);
            print("Object Type = " + firstTile.GetTile().objectType);
            rightLink++;
            if (rightLink == 2)
            {
                print("Match");
            }
            GetNextTileOnGrid(x + 1, y, rightLink);

        }
        else if (rightLink >= 2)
        {
            // add matches to list
            for (int i = tempMatchList[0].tileIndex.x; i < rightLink; i++)
            {
                tempMatchList.Add(grid[i, y]);
                matchList.Add(grid[i, y]);
            }
            print("Matches added to list");
        }
        else
        {
            tempMatchList.Clear();
            print("No matchees at bottom");
        }

        print("Rightlinkcounter: " + rightLink);

    }

    TileSlot GetTile(int x, int y)
    {
        return grid[x, y];
    }
    // Getter Methods

    public float GetCycleLength() => _cycleLength;

    public Vector2Int GetGridSize() => _gridLength;
}
