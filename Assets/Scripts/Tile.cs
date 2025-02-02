using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector3 initialScale;
    public ObjectType objectType;
    public Vector2Int index;
    public TileSlot mySlot;
    private void Start()
    {
        initialScale = transform.localScale;

    }

    public void SetSlot(TileSlot slot)
    {
        mySlot = slot;
        transform.position = slot.transform.position; // Snap to slot position
    }

    

    public void HighlightTile(bool highlight)
    {
        if (highlight)
        {
            transform.localScale = initialScale * 1.2f;
        }
        else
        {
            transform.localScale = initialScale;
        }
    }
}
