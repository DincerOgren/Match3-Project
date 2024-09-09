using UnityEngine;

public class BackgroundHolder : MonoBehaviour
{
    public Tile heldTile;  // The Tile that this backgroundObject is holding

    public TileObject[] tiles;
    public GameObject tilePrefab;

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
        heldTile = spawnedObject.GetComponent<Tile>();
    }
    public void AssignTile(Tile tile)
    {
        heldTile = tile;
        tile.transform.position = this.transform.position;  // Position the tile on top of the background object
        tile.transform.parent = this.transform;             // Set the background as the parent of the tile
    }

    // Optional: You can also implement logic here to check if the tile is removable or empty
    public bool HasTile()
    {
        return heldTile != null;
    }

    public void ClearTile()
    {
        heldTile = null;
    }
}
