using UnityEngine;


public class TileSlot : MonoBehaviour
{
    //TODO: Privatte name convencion;
    public Tile currentTile;

    public Vector2Int tileIndex;

    private void OnMouseDown()
    {
        GameManager.Instance.SelectTile(currentTile);
    }
    public void SetTile(Tile tile)
    {
        currentTile = tile;
        tile.index = tileIndex;
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
    public void DestroyTile()
    {
        // PLAY SFX
        // PLAY VFX

        currentTile.DeleteTile();

        ClearTile();
    }
    public Tile GetTile() => currentTile;
}
