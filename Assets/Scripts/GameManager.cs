using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] float _cycleLength = 2;
    [SerializeField] Vector2Int _gridLength = new(5, 5);


    public bool checkHorizontalMatches = true;
    public bool checkVerticalMatches = true;

    public float moveSpeedAfterSpawn = 0.1f;
    [Header("Touch Block")]
    public GameObject blockObject;
    public bool canSwap = true;

    public Tile tileA;
    public Tile tileB;


    TileSlot[,] grid;

    public List<int> matchAmountForX;
    public List<int> matchAmountForY;

    public List<TileSlot> hMatchList;
    public List<TileSlot> vMatchList;
    public List<TileSlot> tempMatchList;
    public List<TileSlot> tempLList;
    public List<TileSlot> lMatchesList;
    public List<TileSlot> emptyList;

    [Header("Suggest Section")]
    public bool isPlayerInactive = true;
    public bool isCountingToSuggest = true;
    public float suggesTime = 2f;
    public float suggestTimer = 0;
    public List<Tile> suggestRightList;
    public List<Tile> suggestUpList;

    public List<int> suggestRightAmount;
    public List<int> suggestUpAmount;

    public List<Tile> suggestedTiles;

    public float scaleMultiplier = 1.5f;
    public float suggestCycleLength = .5f;
    public bool isAlreadyAnimatingSuggestedTiles = false;
    [Header("Score")]
    public TextMeshProUGUI scoreText;
    public float score = 0;

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

        //UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        print("Score update call");

        scoreText.text = score.ToString();
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
    // Possible matches on start destroy them and replace them V
    // Suggest Matches                              -
    // suggest with time 
    // REPLACE MAP WHEN NO MATCH



    // Special symbols? or spells? 
    // implement magician
    // shoot fireball when match
    // add diff spells for different match counts
    // add health systems
    // add enemy
    // add enemy fight back
    //


    // New Error appeared -> When L Shape includes another horizontal match it is giving null error 
    // Visual ->   x x x z 
    //             y x y z    
    //             y x x x
    //
    // i think i fixed but i have to make this scneario again to test it 
    //
    //
    // TOMORROW TO DO -> Finish the basic game loop and save this project for basic match 3 prototype



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

        //SearchLShape();
        DestroyMethods(true);
        SlideObjectsAfterMatch(true);
        SpawnNewTiles(true);
        ClearLists();

        print("Match Found");
        CheckMatchesOnStart();

    }
    public void SelectTile(Tile tile)
    {

        StopSuggest();
        print("Touch selected tile index = " + tile.index);


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
                canSwap = false;
                StartCoroutine(SwapTiles(tileA, tileB));
            }
            else
                print("Shouldnt Swap");



            tileA.HighlightTile(false);
            tileB.HighlightTile(false);
            tileA = null;
            tileB = null;
            ResetSuggestTimer();

        }


    }

    void ResetSuggestTimer()
    {
        suggestTimer = 0;
    }

    void StopSuggest()
    {
        KillTweens();
        ClearSuggestLists();
        isPlayerInactive = false;
    }
    void SuggestMatchWithTime()
    {

        if (suggestTimer > suggesTime && !isAlreadyAnimatingSuggestedTiles && isPlayerInactive)
        {
            Debug.LogWarning("Suggest Now");
            SuggestMatch();
        }
        else if (canSwap)
        {

            suggestTimer += Time.deltaTime;
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
                //yield return new WaitForSeconds(_cycleLength);
                //aSlot.SetTile(tileA);
                //bSlot.SetTile(tileB);
                //yield return new WaitForSeconds(_cycleLength);
                canSwap = true;
            }
            else
            {
                yield return new WaitForSeconds(moveSpeedAfterSpawn);
                MethodsAfterSwapAndMatch();

                yield return new WaitForSeconds(moveSpeedAfterSpawn);

                StartCoroutine(CheckMatchesAfterNewReplacement());
            }

        }
        //else
        {
            // do nothing
            // swap but return back object;

        }
    }

    void SwapTiles(TileSlot slotA, TileSlot slotB, bool suggestForRight = true)
    {
        if (slotA.GetTile() == null || slotB.GetTile() == null)
        {
            Debug.LogError("Some tiles are empty");
            return;
        }
        var aTile = slotA.GetTile();
        var bTile = slotB.GetTile();

        slotA.SetTile(bTile, true);
        slotB.SetTile(aTile, true);

        if (!CheckMatches())
        {
            slotA.SetTile(aTile, true);
            slotB.SetTile(bTile, true);
            //No matches after switch
            //print("No Matches after switching " + slotA.name + " - " + slotB.name);
            ClearLists();
            return;
        }

        {


            print("Matches found after switching " + slotA.name + " - " + slotB.name + " for right= " + suggestForRight);

            {
                if (hMatchList.Contains(slotA))
                {
                    print("SLOT A FOR H");
                    CheckSuggestMatch(slotA, hMatchList, matchAmountForX, suggestRightList);

                }
                if (vMatchList.Contains(slotA))
                {
                    print("SLOT A FOR V");

                    CheckSuggestMatch(slotA, vMatchList, matchAmountForY, suggestUpList, true);

                }

                if (hMatchList.Contains(slotB))
                {
                    print("SLOT B FOR H");

                    CheckSuggestMatch(slotB, hMatchList, matchAmountForX, suggestRightList);

                }
                if (vMatchList.Contains(slotB))
                {
                    print("SLOT B FOR V");

                    CheckSuggestMatch(slotB, vMatchList, matchAmountForY, suggestUpList, true);

                }


            }
            //else
            //{
            //    if (hMatchList.Contains(slotA) || vMatchList.Contains(slotA))
            //    {

            //        suggestUpList.Add(slotA);
            //    }
            //    if (hMatchList.Contains(slotB) || vMatchList.Contains(slotB))
            //    {

            //        suggestUpList.Add(slotB);

            //    }
            //}

            slotA.SetTile(aTile, true);
            slotB.SetTile(bTile, true);
            ClearLists();
        }
    }

    private void CheckSuggestMatch(TileSlot slot, List<TileSlot> slotList, List<int> amountList, List<Tile> toAddList, bool forUpLink = false)
    {

        int matchAmount = 0;

        var tile = slot.GetTile();
        if (forUpLink)
        {
            int index = slotList.IndexOf(slot);
            int xIndex = slot.tileIndex.x;
            int temp = 0;
            print("Before for index =" + index);
            for (int i = 0; i < amountList.Count; i++)
            {
                temp += amountList[i];
                print("in for temp = " + temp + " amount count = " + amountList.Count);

                if (temp > index)
                {
                    temp -= amountList[i];

                    //int yIndexStart = 0;
                    //if (i+1==amountList.Count && temp !=0)
                    //{
                    //    yIndexStart = slotList[temp].tileIndex.y;
                    //    Debug.LogWarning("Last Object with temp = "+ temp);
                    //}
                    //else
                    int yIndexStart = slotList[temp].tileIndex.y;

                    for (int j = 0; j < amountList[i]; j++)
                    {
                        //if (grid[xIndex, temp + j].GetTile().objectType != tile.objectType)
                        {
                            print("originalSlotsTile = " + slot.GetTile().objectType);
                            // print("slots tile = " + xIndex + "," + temp + j + " tiles type = " + grid[xIndex, temp + j].GetTile().objectType);
                            //toAddList.Add(tile);
                            //continue;
                        }
                        matchAmount++;
                        print("Temp = " + temp + " and J =" + j + " for x" + " index = " + index + " xindex = " + xIndex + " yIndex = " + yIndexStart);
                        // 

                        //this not working either toAddList.Add(slotList[temp + j].GetTile());
                        //toAddList.Add(grid[xIndex, temp + j].GetTile());
                        toAddList.Add(grid[xIndex, yIndexStart + j].GetTile());
                    }
                    suggestUpAmount.Add(matchAmount);
                    matchAmount = 0;
                    break; //?
                }

            }

        }
        else
        {
            int index = slotList.IndexOf(slot);
            int yIndex = slot.tileIndex.y;
            int temp = 0;
            for (int i = 0; i < amountList.Count; i++)
            {
                temp += amountList[i];
                if (temp > index)
                {

                    temp -= amountList[i];
                    int xIndexStart = slotList[temp].tileIndex.x;
                    for (int j = 0; j < amountList[i]; j++)
                    {
                        //if (grid[temp + j, yIndex].GetTile().objectType != tile.objectType)
                        {
                            print("originalSlotsTile = " + slot.GetTile().objectType);
                            // print("slots tile = " + yIndex + "," + temp + j + " tiles type = " + grid[temp + j, yIndex].GetTile().objectType);
                        }
                        print("Temp = " + temp + " and J =" + j + " for x" + " index = " + index + " yindex = " + yIndex + "xIndex = " + xIndexStart);

                        //toAddList.Add(slotList[temp + j].GetTile());
                        //toAddList.Add(grid[temp + j, yIndex].GetTile());
                        toAddList.Add(grid[xIndexStart + j, yIndex].GetTile());

                        matchAmount++;
                    }

                    suggestRightAmount.Add(matchAmount);
                    matchAmount = 0;
                    break;
                }
            }
        }

        //ChooseRandomMatchToAnimate();
        //SuggestAnimation();

    }

    private void ChooseRandomMatchToAnimate()
    {
        int random = UnityEngine.Random.Range(1, 3);

        if (suggestUpList.Count < 3 && suggestRightList.Count < 3)
        {
            Debug.LogWarning("No suggestion in here");
            return;
        }

        // Only right list includes matches
        if (suggestUpList.Count < 3)
        {
            Debug.LogWarning("Going iwth right list cause no match on uplist");
            // Go with right list
            Tile[] matchTiles;
            if (suggestRightAmount.Count > 1)
            {
                //Contains more than 1 match select random match

                int randomMatchStart = UnityEngine.Random.Range(0, suggestRightAmount.Count);
                Debug.LogWarning("Right list contains more than 1 match random = " + randomMatchStart);
                int temp = 0;

                matchTiles = new Tile[suggestRightAmount[randomMatchStart]];

                for (int i = 0; i < suggestRightAmount.Count; i++)
                {

                    if (i == randomMatchStart)
                    {
                        for (int j = 0; j < suggestRightAmount[i]; j++)
                        {
                            matchTiles[j] = suggestRightList[j + temp];
                        }

                        break;
                    }
                    temp += suggestRightAmount[i];
                }


                SuggestAnimation(matchTiles);
            }
            //only contains 1 match so go with full list
            else
            {
                Debug.LogWarning("Right list only contains 1 match");

                matchTiles = new Tile[suggestRightList.Count];

                for (int j = 0; j < suggestRightList.Count; j++)
                {
                    matchTiles[j] = suggestRightList[j];
                }

                SuggestAnimation(matchTiles);

            }

            return;
        }

        // Only right list includes matches
        if (suggestRightList.Count < 3)
        {
            Debug.LogWarning("Going iwth up list cause no match on rightlist");

            // Go with up list
            Tile[] matchTiles;
            if (suggestUpAmount.Count > 1)
            {
                //Contains more than 1 match select random match

                int randomMatchStart = UnityEngine.Random.Range(0, suggestUpAmount.Count);
                Debug.LogWarning("UP list contains more than 1 match random = " + randomMatchStart);

                int temp = 0;

                matchTiles = new Tile[suggestUpAmount[randomMatchStart]];

                for (int i = 0; i < suggestUpAmount.Count; i++)
                {

                    if (i == randomMatchStart)
                    {
                        for (int j = 0; j < suggestUpAmount[i]; j++)
                        {
                            matchTiles[j] = suggestUpList[j + temp];
                        }

                        break;
                    }
                    temp += suggestUpAmount[i];
                }


                SuggestAnimation(matchTiles);
            }
            //only contains 1 match so go with full list
            else
            {
                Debug.LogWarning("UP list only contains 1 match");

                matchTiles = new Tile[suggestUpList.Count];

                for (int j = 0; j < suggestUpList.Count; j++)
                {
                    matchTiles[j] = suggestUpList[j];
                }

                SuggestAnimation(matchTiles);

            }

            return;
        }

        // Both lists are contains some matches we can choose random list
        if (random == 1)
        {
            print("RANDOM == 1");
            //Suggest in up link
            Tile[] matchTiles;
            if (suggestUpAmount.Count > 1)
            {
                //Contains more than 1 match select random match

                int randomMatchStart = UnityEngine.Random.Range(0, suggestUpAmount.Count);
                Debug.LogWarning("UP list contains more than 1 match random = " + randomMatchStart);

                int temp = 0;

                matchTiles = new Tile[suggestUpAmount[randomMatchStart]];

                for (int i = 0; i < suggestUpAmount.Count; i++)
                {

                    if (i == randomMatchStart)
                    {
                        for (int j = 0; j < suggestUpAmount[i]; j++)
                        {
                            matchTiles[j] = suggestUpList[j + temp];
                        }

                        break;
                    }
                    temp += suggestUpAmount[i];
                }


                SuggestAnimation(matchTiles);
            }
            //only contains 1 match so go with full list
            else
            {
                Debug.LogWarning("UP list contains 1 match");

                matchTiles = new Tile[suggestUpList.Count];

                for (int j = 0; j < suggestUpList.Count; j++)
                {
                    matchTiles[j] = suggestUpList[j];
                }

                SuggestAnimation(matchTiles);

            }


        }
        else if (random == 2)
        {
            print("RANDOM == 2");
            // suggest in rightlink

            Tile[] matchTiles;
            if (suggestRightAmount.Count > 1)
            {
                //Contains more than 1 match select random match

                int randomMatchStart = UnityEngine.Random.Range(0, suggestRightAmount.Count);
                Debug.LogWarning("Right list contains more than 1 match random = " + randomMatchStart);

                int temp = 0;
                matchTiles = new Tile[suggestRightAmount[randomMatchStart]];

                for (int i = 0; i < suggestRightAmount.Count; i++)
                {

                    if (i == randomMatchStart)
                    {
                        for (int j = 0; j < suggestRightAmount[i]; j++)
                        {
                            matchTiles[j] = suggestRightList[j + temp];
                        }

                        break;
                    }
                    temp += suggestRightAmount[i];
                }


                SuggestAnimation(matchTiles);
            }
            //only contains 1 match so go with full list
            else
            {
                Debug.LogWarning("Right list contains 1 match");

                matchTiles = new Tile[suggestRightList.Count];

                for (int j = 0; j < suggestRightList.Count; j++)
                {
                    matchTiles[j] = suggestRightList[j];
                }

                SuggestAnimation(matchTiles);

            }

            return;
        }
        else
            Debug.LogError("Something wrong with random = " + random);
    }

    private void SuggestAnimation(Tile[] tilesToAnimate)
    {
        if (isAlreadyAnimatingSuggestedTiles)
        {
            Debug.LogWarning("Already animating tiles");
            return;
        }
        for (int i = 0; i < tilesToAnimate.Length; i++)
        {
            tilesToAnimate[i].transform.DOScale(scaleMultiplier, suggestCycleLength).SetLoops(-1, LoopType.Yoyo).SetId("SuggestTween");
            suggestedTiles.Add(tilesToAnimate[i]);
        }
        isAlreadyAnimatingSuggestedTiles = true;
    }

    IEnumerator CheckMatchesAfterNewReplacement()
    {
        if (!CheckMatches())
        {
            //Match yok
            print("NO MATCH AFTER REPLACE");
            isPlayerInactive = true;
            canSwap = true;
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

    void SuggestMatch()
    {


        for (int y = 0; y < _gridLength.y; y++)
        {
            for (int x = 0; x < _gridLength.x; x++)
            {
                if (x + 1 >= _gridLength.x && y + 1 >= _gridLength.y)
                {
                    break;
                }

                if (x + 1 >= _gridLength.x)
                {
                    SwapTiles(grid[x, y], grid[x, y + 1], false);
                    continue;
                }

                if (y + 1 >= _gridLength.y)
                {
                    SwapTiles(grid[x, y], grid[x + 1, y]);
                    continue;
                }


                // Check Right First
                SwapTiles(grid[x, y], grid[x + 1, y]);
                // Then Check Up
                SwapTiles(grid[x, y], grid[x, y + 1], false);


            }

        }
        ChooseRandomMatchToAnimate();

    }

    private void KillTweens()
    {
        DOTween.KillAll();
        foreach (var item in suggestedTiles)
        {
            item.transform.localScale = Vector3.one;
        }
    }

    private void ClearSuggestLists()
    {
        isAlreadyAnimatingSuggestedTiles = false;
        suggestUpList.Clear();
        suggestRightList.Clear();
        suggestRightAmount.Clear();
        suggestUpAmount.Clear();
        suggestedTiles.Clear();
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
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    print("Try matches = " + CheckMatches());
        //    //CheckMatches();
        //}

        SuggestMatchWithTime();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            KillTweens();
            ClearSuggestLists();

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckMatchesOnStart();
        }


        if (Input.GetKeyDown(KeyCode.S))
        {
            ClearLists();
        }
    }

    private void DestroyMethods(bool isStart = false)
    {
        if (isStart)
        {

            DestroyHorizontalNLShape();
            DestroyVerticals();
        }
        else
        {
            CalculateHorizontalPoints();
            DestroyHorizontalNLShape();
            CalculateVerticalPoints();
            DestroyVerticals();
        }
    }

    void CalculateHorizontalPoints()
    {
        if (hMatchList.Count < 0) 
        {
            print("No Horizontal matches");
            return;
        }
        float tempPoints = 0;
        int temp = 0;
        for (int i = 0; i < matchAmountForX.Count; i++)
        {
            tempPoints += hMatchList[temp].GetTile().tilePoint * matchAmountForX[i];
            print("Horizontal calculate tempPoints = " + tempPoints + " matchamountX i =" + matchAmountForX[i]+ " i = "+i);


            temp += matchAmountForX[i];
            
        }

        score += tempPoints;

        UpdateScoreText();
    }

    void CalculateVerticalPoints()
    {
        if(vMatchList.Count <= 0)
        {
            print("Vertical match points deleted cause of L Match");
            return;
        }
        float tempPoints = 0;
        int temp = 0;
        for (int i = 0; i < matchAmountForY.Count; i++)
        {
            tempPoints += vMatchList[temp].GetTile().tilePoint * matchAmountForY[i];
            print("Vertical calculate tempPoints = " + tempPoints);
            temp += matchAmountForY[i];

        }

        score += tempPoints;

        UpdateScoreText();
    }
    private void ClearLists()
    {
        hMatchList.Clear();
        lMatchesList.Clear();
        vMatchList.Clear();
        matchAmountForX.Clear();
        matchAmountForY.Clear();
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

                        matchAmountForX.Add(rightLink + 1);

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
                        matchAmountForY.Add(upLink + 1);
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
                    if (lMatchesList.Contains(grid[currentSlot.tileIndex.x, i]))
                    {
                        break;
                    }
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
                    if (lMatchesList.Contains(grid[currentSlot.tileIndex.x, i]))
                    {
                        break;
                    }
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
                for (int i = 0; i < matchAmountForX.Count; i++)
                {
                    insertIndex += matchAmountForX[i];
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

                        // Should delete same tiles from list 

                        for (int i = 0; i < lMatchesList.Count; i++)
                        {

                            if (hMatchList.Contains(lMatchesList[i]))
                            {
                                int indexOfSameTile = hMatchList.IndexOf(lMatchesList[i]);
                                int tempIndex = 0;
                                for (int j = 0; j < matchAmountForX.Count; j++)
                                {
                                    tempIndex += matchAmountForX[i];
                                    if (tempIndex>indexOfSameTile)
                                    {
                                        Debug.LogWarning("We Removed tile from hList = " + lMatchesList[i].tileIndex.x + "," + lMatchesList[i].tileIndex.y);
                                        hMatchList.RemoveAt(indexOfSameTile);
                                        matchAmountForX[i] -= 1;
                                    }
                                }
                            }
                        }
                        hMatchList.InsertRange(insertIndex, lMatchesList);

                        matchAmountForX[index] += upLinks + downLinks;


                        tempLList.Clear();
                        k += matchAmountForX[index];
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
                            grid[x, tempY].SetTile(grid[x, j].GetTile(), true);
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
                        grid[x, i].SetTile(spawnedTiles[tileCounter], true);

                    }
                    else
                    {
                        grid[x, i].SetTile(spawnedTiles[tileCounter], false, moveSpeedAfterSpawn);

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
    public TileSlot GetTileSlot(int x, int y)
    {
        return grid[x, y];
    }
    // Getter Methods

    public float GetCycleLength() => _cycleLength;

    public bool CanSwap() => canSwap;
    public Vector2Int GetGridSize() => _gridLength;
}
