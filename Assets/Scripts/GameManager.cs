using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public Tile tileA;

    public Tile tileB;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
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
               // SwapTiles();
            }
            else
                print("Shouldnt Swap");



            tileA.HighlightTile(false);
            tileB.HighlightTile(false);
            tileA = null;
            tileB = null;
        }

        
    }

    private void SwapTiles()
    {
        throw new NotImplementedException();
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


}
