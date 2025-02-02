using UnityEngine;


public class TileSlot : MonoBehaviour
{
    public Tile currentTile;

    public Vector2Int tileNum;

    private void OnMouseDown()
    {
        GameManager.Instance.SelectTile(currentTile);
    }
    public void AssignTile(Tile tile)
    {
        currentTile = tile;
        tile.SetSlot(this); // Let the tile know its slot
    }

    public bool IsEmpty()
    {
        return currentTile == null;
    }

    public void ClearTile()
    {
        currentTile = null; // Tile is removed (e.g., after a match)
    }
}
