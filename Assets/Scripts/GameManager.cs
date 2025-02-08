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

    public float moveSpeedAfterSpawn = 0.1f;

    public Tile tileA;
    public Tile tileB;


    TileSlot[,] grid;

    public List<int> matchQuantity;

    public List<TileSlot> hMatchList;
    public List<TileSlot> vMatchList;
    public List<TileSlot> tempMatchList;
    public List<TileSlot> tempLList;
    public List<TileSlot> lMatchesList;
    public List<TileSlot> emptyList;

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

        CheckMatchesOnStart();
    }




    #region GAME LOOP

    // SWAP                                         V
    // DETECT MATCH                                 V
    // DESTROY MATCHES                              V
    // REPLACE MATCHES WITH NULL                    V
    // SLIDE ABOVE TILES                            V
    // SPAWN NEW TILES FOR EMPTY SLOTS              V
    // SLIDE THEM DOWN TOO                          V
    // Matches on Start                             V
    // Special symbols? or spells? 
    // implement magician
    // shoot fireball when match
    // add diff spells for different match counts
    // add health systems
    // add enemy
    // add enemy fight back
    //



    // Possible matches on start destroy them and replace them V


    /*
     * 1- Check Matches
     * 2- Search L Shape
     * 3- Destroy Objects
     * 4- Slide above objects
     * 5- Spawn new and replace empty slots
     * 6- Clear Lists
     * 
     */

    #endregion

    private void CheckMatchesOnStart()
    {
        if (!CheckMatches())
        {
            print("No matches in start ");
            return;
        }

        SearchLShape();
        DestroyMethods();
        SlideObjectsAfterMatch(true);
        SpawnNewTiles(true);
        ClearLists();

        print("Match Found");
        CheckMatchesOnStart();

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
                StartCoroutine(SwapTiles(tileA, tileB));
            }
            else
                print("Shouldnt Swap");



            tileA.HighlightTile(false);
            tileB.HighlightTile(false);
            tileA = null;
            tileB = null;
        }


    }

    private IEnumerator SwapTiles(Tile tileA, Tile tileB)
    {
        // Check if they are same objects
        //inside swap
        //if (tileA.objectType == tileB.objectType)
        {

            var aSlot = tileA.GetSlot();
            var bSlot = tileB.GetSlot();



            // get temp pos = a
            // a = b
            ////b = temp
            //tileA.transform.DOMove(tileB.GetSlot().transform.position, _cycleLength).SetEase(Ease.InOutSine);
            //tileB.transform.DOMove(aPos, _cycleLength).SetEase(Ease.InOutSine);

            aSlot.SetTile(tileB);
            bSlot.SetTile(tileA);

            if (!CheckMatches())
            {
                //Return back objects
                print("Return  back objects");

                // shgould add delay
                yield return new WaitForSeconds(_cycleLength);
                aSlot.SetTile(tileA);
                bSlot.SetTile(tileB);
            }
            else
            {
                yield return new WaitForSeconds(_cycleLength);
                MethodsAfterSwapAndMatch();

                yield return new WaitForSeconds(_cycleLength);

                StartCoroutine(CheckMatchesAfterNewReplacement());
            }

        }
        //else
        {
            // do nothing
            // swap but return back object;

        }
    }

    IEnumerator CheckMatchesAfterNewReplacement()
    {
        if (!CheckMatches())
        {
            //Match yok
            print("NO MATCH AFTER REPLACE");
            yield break;
        }
        else
        {
            print("Match after replace");
            yield return new WaitForSeconds(_cycleLength);
            MethodsAfterSwapAndMatch();
            yield return new WaitForSeconds(_cycleLength);
            StartCoroutine(CheckMatchesAfterNewReplacement());
        }


        //CheckMatchesAfterNewReplacement();
    }
    private void MethodsAfterSwapAndMatch()
    {
        SearchLShape();
        DestroyMethods();
        SlideObjectsAfterMatch();
        SpawnNewTiles();
        ClearLists();
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
            print("Try matches = " + CheckMatches());
            //CheckMatches();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SearchLShape();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            CheckMatchesAfterNewReplacement();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SlideObjectsAfterMatch();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnNewTiles();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DestroyMethods();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ClearLists();
        }
    }

    private void DestroyMethods()
    {
        DestroyHorizontalNLShape();
        DestroyVerticals();
    }

    private void ClearLists()
    {
        hMatchList.Clear();
        lMatchesList.Clear();
        vMatchList.Clear();
        matchQuantity.Clear();
    }

    bool CheckMatches()
    {
        bool horizontalMatch = false;
        bool verticalMatch = false;
        TileSlot firstTile;
        if (checkHorizontalMatches)
        {
            //horizontal

            int rightLink = 0;
            //length -2 ?
            for (int y = 0; y < _gridLength.y; y++)
            {
                //print("in most up for :" + y);
                int row = 0;

                while (row <= _gridLength.x - 2)
                {
                    //print("in while row : " + row);

                    firstTile = grid[row, y];

                    if (hMatchList.Contains(firstTile))
                    {
                        row++;
                        continue;
                    }

                    tempMatchList.Add(firstTile);

                    for (int i = row + 1; i < _gridLength.x; i++)
                    {
                        //print("in for : " + i);
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
                        //print("match found after for");
                        horizontalMatch = true;
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
                // print("in most up for vertical :" + x);
                int column = 0;

                while (column <= _gridLength.y - 2)
                {
                    //   print("checking vertical matches for grid[" + x + "," + column + "]");

                    firstTile = grid[x, column];

                    if (vMatchList.Contains(firstTile))
                    {
                        column++;
                        continue;
                    }

                    tempMatchList.Add(firstTile);

                    for (int i = column + 1; i < _gridLength.y; i++)
                    {
                        // print("in vertical for : " + i);
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
                        //print("vertical match found after for");
                        vMatchList.AddRange(tempMatchList);
                        verticalMatch = true;
                    }

                    upLink = 0;
                    tempMatchList.Clear();

                    column++;
                }

            }
        }


        return horizontalMatch || verticalMatch;
    }

    private void DestroyHorizontalNLShape()
    {
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
            upLinks = 0;
            downLinks = 0;
            //find up links 
            TileSlot currentSlot = hMatchList[k];

            //print("while k = " + k + " Tile is = " + currentSlot.name);

            for (int i = currentSlot.tileIndex.y + 1; i <= _gridLength.y - 1; i++)
            {
                //print("i = " + i + " calculated value = " + (_gridLength.y - currentSlot.tileIndex.y));

                if (grid[currentSlot.tileIndex.x, i].GetTile().objectType == currentSlot.GetTile().objectType)
                {
                    upLinks++;
                    tempLList.Add(grid[currentSlot.tileIndex.x, i]);
                  //  print(currentSlot.name + " icin " + "uplink bulundu adi : " + grid[currentSlot.tileIndex.x, i].name);
                }
                else
                    break;
            }

            //find down links

            for (int i = currentSlot.tileIndex.y - 1; i >= 0; i--)
            {
                //print("i = " + i + " on downliinkm");

                if (grid[currentSlot.tileIndex.x, i].GetTile().objectType == currentSlot.GetTile().objectType)
                {
                    downLinks++;
                    tempLList.Add(grid[currentSlot.tileIndex.x, i]);
                    //print(currentSlot.name + " icin " + "downlink bulundu adi : " + grid[currentSlot.tileIndex.x, i].name);

                }
                else
                    break;
            }

            if (upLinks + downLinks >= 2)
            {
                //print("We find a L shape match");
                lMatchesList = new(tempLList);


                int insertIndex = 0;
                int index = 0;

                // Calculating where to insert L Shapes in matchlist 
                for (int i = 0; i < matchQuantity.Count; i++)
                {
                    insertIndex += matchQuantity[i];
                    if (insertIndex > k)
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
                        //Debug.Log($"Adding {lMatchesList[0].name} to hMatchList.");

                        hMatchList.InsertRange(insertIndex, lMatchesList);

                        matchQuantity[index] += upLinks + downLinks;

                        tempLList.Clear();
                        k += matchQuantity[index];
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

    void SlideObjectsAfterMatch(bool shouldSetInstant = false)
    {
        List<TileSlot> aboveList = new();

        for (int x = 0; x < _gridLength.x; x++)
        {
            for (int y = 0; y < _gridLength.y; y++)
            {
                // Check if tile is null
                if (grid[x, y].GetTile() == null)
                {
                    if (aboveList.Contains(grid[x, y]))
                    {
                        continue;
                    }

                    aboveList.Add(grid[x, y]);

                    int startY = y + 1;
                    while (startY < _gridLength.y)
                    {
                        if (grid[x, startY].GetTile() != null)
                        {
                            break;
                        }
                        else
                            aboveList.Add(grid[x, startY]);

                        startY++;
                    }
                    //print("Should move grid[" + x + "," + startY + "] to grid " + x + "," + y);

                    int j = startY;
                    int tempY = y;
                    while (j < _gridLength.y)
                    {
                        //if (j+1>_gridLength.y)
                        //{
                        //    //LAST ROW
                        //}

                        // NULL ERROR
                        // its probably cause a null object then null again while checking downwards to upwards
                        if (grid[x, j].GetTile() == null)
                        {
                            j++;
                            continue;
                        }
                        if (shouldSetInstant)
                        {
                            grid[x, tempY].SetTile(grid[x, j].GetTile(), 0, true);
                        }
                        else
                            grid[x, tempY].SetTile(grid[x, j].GetTile());

                        grid[x, j].ClearTile();


                        tempY++;
                        j++;
                    }
                    //List<Tile> spawnedTiles = GridManager.Instance.SpawnTile(x, startY - y);

                    //for (int i = 0; i < length; i++)
                    //{

                    //}
                }
            }
            aboveList.Clear();

        }
    }

    void SpawnNewTiles(bool shouldSpawnInstant = false)
    {
        int emptyAmount = 0;
        int tempY = 0;
        for (int x = 0; x < _gridLength.x; x++)
        {
            for (int y = 0; y < _gridLength.y; y++)
            {
                // Check if tile is null
                if (grid[x, y].GetTile() == null)
                {
                    //print("First null at " + x + "," + y + " name = " + grid[x, y]);
                    if (emptyList.Contains(grid[x, y]))
                    {
                        //print("Continue");
                        continue;
                    }
                    emptyList.Add(grid[x, y]);
                    emptyAmount++;

                    tempY = y + 1;
                    while (tempY < _gridLength.y)
                    {
                       // print("in while"); ;
                        if (grid[x, tempY].GetTile() == null)
                        {
                          //  print("Added in while tilename: " + grid[x, tempY]);
                            emptyList.Add(grid[x, tempY]);
                            emptyAmount++;

                        }
                        // WE MAKE SURE Execute this line after sliding all objects above
                        else
                            print("ONE OBJECT IS NOT NULL SOMETHING WRONG");
                        tempY++;
                    }
                }
                else
                    continue;


                //print("x = " + x + " empty count = " + emptyAmount);
                List<Tile> spawnedTiles = new();
                spawnedTiles = GridManager.Instance.SpawnTile(x, emptyAmount);
                //print("SPAWNED TILES COUNT = " + spawnedTiles.Count);
                int tileCounter = 0;
                for (int i = y; i < _gridLength.y; i++)
                {
                  //  print("in for i= " + i);
                    if (grid[x, i].GetTile() != null)
                    {
                        Debug.LogError("Cant set cause its already filled up grid " + x + "," + y);
                        return;
                    }

                    if (spawnedTiles[tileCounter] == null)
                    {
                        Debug.LogError("Counter not working");
                    }

                    if (shouldSpawnInstant)
                    {
                        grid[x, i].SetTile(spawnedTiles[tileCounter], 0, true);

                    }
                    else
                    {
                        grid[x, i].SetTile(spawnedTiles[tileCounter], moveSpeedAfterSpawn);

                    }
                   // print("Setted grid " + x + "," + y);

                    tileCounter++;

                }

            }

            emptyAmount = 0;
            emptyList.Clear();
            tempY = 0;
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
