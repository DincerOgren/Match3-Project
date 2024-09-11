using Unity.VisualScripting;
using UnityEngine;

public class BackgroundHolder : MonoBehaviour
{
    public Vector2Int arrayIndex;
    public Tile heldTile;  // The Tile that this backgroundObject is holding

    public TileObject[] tiles;
    public GameObject tilePrefab;

    public Transform objectParent;
    private void Start()
    {
        Initalize();
    }
    
    void Initalize()
    {
        int tileToUse = Random.Range(0, tiles.Length);
        TileObject tile = tiles[tileToUse];
        GameObject spawnedObject = Instantiate(tilePrefab, transform.position, Quaternion.identity);
        SetTile(tile,spawnedObject);
    }

    public void SetTile(TileObject tile,GameObject spawnedObject)
    {
        spawnedObject.GetComponent<SpriteRenderer>().sprite = tile.sprite;
        spawnedObject.GetComponent<SpriteRenderer>().color = tile.color;
        spawnedObject.name = tile.name;
        spawnedObject.GetComponent<Tile>().objectType=tile.objectType;
        spawnedObject.transform.parent=objectParent;
        heldTile = spawnedObject.GetComponent<Tile>();
        heldTile.index = arrayIndex;

        GameManager.Instance.grid[arrayIndex.x,arrayIndex.y] = heldTile;
       // GameManager.Instance.SetGrid(arrayIndex, heldTile);
    }
  

    public bool HasTile()
    {
        return heldTile != null;
    }

    public void ClearTile()
    {
        heldTile = null;
    }
}
