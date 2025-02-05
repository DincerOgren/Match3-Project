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


    public bool checkHorizontalMatches = true;
    public bool checkVerticalMatches = true;


    public Tile tileA;
    public Tile tileB;


    TileSlot[,] grid;

    public List<int> matchQuantity;

    public List<TileSlot> hMatchList;
    public List<TileSlot> vMatchList;
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


    #region GAME LOOP

    // SWAP                                         V
    // DETECT MATCH                                 V
    // DESTROY MATCHES                              V
    // REPLACE MATCHES WITH NULL                    V
    // SLIDE ABOVE TILES                            V
    // SPAWN NEW TILES FOR EMPTY SLOTS              
    // SLIDE THEM DOWN TOO                          



    #endregion
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



            CheckMatches();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SearchLShape();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SlideObjectsAfterMatch();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DestroyHorizontalNLShape();
            DestroyVerticals();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            hMatchList.Clear();
            lMatchesList.Clear();
            vMatchList.Clear();
            matchQuantity.Clear();
        }
    }
    //private void CheckMatches()
    //{

    //    // Horizontal Match
    //    //int rightMatchCounter = 0;


    //    for (int y = 0; y < 1; y++)
    //    {

    //        for (int x = 0; x < _gridLength.x; x++)
    //        {
    //            var firstTile = grid[x, y];
    //            if (x + 1 >= _gridLength.x)
    //            {
    //                print("continue");
    //                continue;
    //            }


    //            // GetNextTileOnGrid(firstTile.tileNum.x, firstTile.tileNum.y, 0);


    //        }
    //    }
    //}

    void CheckMatches()
    {
        TileSlot firstTile;
        if (checkHorizontalMatches)
        {
            //horizontal

            int rightLink = 0;
            //length -2 ?
            for (int y = 0; y < _gridLength.y; y++)
            {
                print("in most up for :" + y);
                int row = 0;

                while (row <= _gridLength.x - 2)
                {
                    print("in while row : " + row);

                    firstTile = grid[row, y];

                    if (hMatchList.Contains(firstTile))
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
                        hMatchList.AddRange(tempMatchList);

                        matchQuantity.Add(rightLink + 1);

                    }

                    rightLink = 0;
                    tempMatchList.Clear();

                    row++;
                }

            }
        }

        //SearchLShape();

        //DestroyHorizontalNLShape();

        // Where to detect L Shapes ?

        int upLink = 0;

        if (checkVerticalMatches)
        {

            //Vertical 
            for (int x = 0; x < _gridLength.x; x++)
            {
                print("in most up for vertical :" + x);
                int column = 0;

                while (column <= _gridLength.y - 2)
                {
                    print("checking vertical matches for grid[" + x + "," + column + "]");

                    firstTile = grid[x, column];

                    if (vMatchList.Contains(firstTile))
                    {
                        column++;
                        continue;
                    }

                    tempMatchList.Add(firstTile);

                    for (int i = column + 1; i < _gridLength.y; i++)
                    {
                        print("in vertical for : " + i);
                        if (firstTile.GetTile().objectType == grid[x, i].GetTile().objectType)
                        {
                            tempMatchList.Add(grid[x, i]);
                            upLink++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (upLink >= 2)
                    {
                        print("vertical match found after for");
                        vMatchList.AddRange(tempMatchList);

                    }

                    upLink = 0;
                    tempMatchList.Clear();

                    column++;
                }

            }
        }

    }

    private void DestroyHorizontalNLShape()
    {
        // DELETE FROM V LIST TOO 
        //if (vMatchList.Contains(hMatchList[i]))
        //{
        // LIKE THIS
        //}

        for (int i = 0; i < hMatchList.Count; i++)
        {
            //MAYBE DO THIS INSIDE LSHAPESEARCH ???
            if (vMatchList.Contains(hMatchList[i]))
            {
                vMatchList.Remove(hMatchList[i]);
            }
            hMatchList[i].DestroyTile();
        }
    }

    void DestroyVerticals()
    {
        for (int i = 0; i < vMatchList.Count; i++)
        {
            vMatchList[i].DestroyTile();
        }
    }
    void SearchLShape()
    {
        if (hMatchList.Count == 0)
        {
            print("Matchlist empty");
            return;
        }

        int k = 0;
        int upLinks = 0;
        int downLinks = 0;
        // Cycle through every tile and find up and down links
        while (k < hMatchList.Count)
        {
            print("while k = " + k);
            upLinks = 0;
            downLinks = 0;
            //find up links 
            TileSlot currentSlot = hMatchList[k];
            for (int i = currentSlot.tileIndex.y + 1; i <= _gridLength.y -1; i++)
            {
                print("i = " + i + " calculated value = " + (_gridLength.y - currentSlot.tileIndex.y));

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


                int insertIndex = 0;
                int index = 0;
                
                // Calculating where to insert L Shapes in matchlist 
                for (int i = 0; i < matchQuantity.Count; i++)
                {
                    insertIndex += matchQuantity[i];
                    if (insertIndex >= k)
                    {
                        index = i;
                        break;
                        // AYNI ANDA 2 L SENARYOSU KONTROL ET? V Calýsýyor
                    }
                }
                
                // Safety checks before adding
                if (lMatchesList.Count > 0 && lMatchesList[0] != null)
                {
                    if (!hMatchList.Contains(lMatchesList[0])) // Prevent infinite loop
                    {
                        Debug.Log($"Adding {lMatchesList[0].name} to hMatchList.");
                        
                        hMatchList.InsertRange(insertIndex, lMatchesList);

                        matchQuantity[index] += upLinks + downLinks;

                        tempLList.Clear();
                        k += matchQuantity[index];
                        k++;
                        continue;
                    }
                    else
                    {
                        //skip
                        Debug.LogError("Attempted to add a duplicate item.");
                    }
                }
                else
                {
                    Debug.LogError("lMatchesList is empty or contains null elements.");
                }


                // UPDATE MATCH QUANTITY ?
                //Remove from verticalList?
            }

            tempLList.Clear();

            k++;

        }
    }

    void SlideObjectsAfterMatch()
    {
        List<TileSlot> nullList = new();

        for (int x = 0; x < _gridLength.x; x++)
        {
            for (int y = 0; y < _gridLength.y; y++)
            {
                // Check if tile is null
                if (grid[x,y].GetTile() == null)
                {
                    if (nullList.Contains(grid[x,y]))
                    {
                        continue;
                    }

                    nullList.Add(grid[x, y]);

                    int startY = y+1;
                    while (startY < _gridLength.y)
                    {
                        if (grid[x, startY].GetTile() != null)
                        {
                            break;
                        }
                        else
                            nullList.Add(grid[x,startY]);
                        
                        startY++;
                    }
                    print("Should move grid[" + x + "," + startY + "] to grid " + x + "," + y);

                    int j = startY;
                    int tempY = y;
                    while(j<_gridLength.y)
                    {
                        //if (j+1>_gridLength.y)
                        //{
                        //    //LAST ROW
                        //}
                        

                        grid[x, tempY].SetTile(grid[x,j].GetTile());
                        grid[x, j].ClearTile();
                        tempY++;
                        j++;
                    }
                }                
            }
        }
    }
    
    TileSlot GetTile(int x, int y)
    {
        return grid[x, y];
    }
    // Getter Methods

    public float GetCycleLength() => _cycleLength;

    public Vector2Int GetGridSize() => _gridLength;
}
