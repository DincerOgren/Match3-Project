using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    private Vector3 initialScale;
    public ObjectType objectType;
    public Vector2Int index;
    public TileSlot mySlot;
    private float _cycleLength;
    private void Start()
    {
        initialScale = transform.localScale;
        _cycleLength = GameManager.Instance.GetCycleLength();
    }

    public void SetSlot(TileSlot slot)
    {
        mySlot = slot;
        // transform.position = slot.transform.position; // Snap to slot position
        transform.DOMove(slot.transform.position, _cycleLength).SetEase(Ease.OutSine);
    } 
    public void AssignSlot(TileSlot slot)
    {
        mySlot = slot;
         transform.position = slot.transform.position; // Snap to slot position
        //transform.DOMove(slot.transform.position, _cycleLength).SetEase(Ease.OutSine);
    }

    public TileSlot GetSlot() => mySlot;

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
