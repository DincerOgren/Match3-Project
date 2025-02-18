using UnityEngine;
using DG.Tweening;
using System;

public class Tile : MonoBehaviour
{
    private Vector3 initialScale;
    public ObjectType objectType;
    public Vector2Int index;
    public TileSlot mySlot;
    private float _cycleLength;
    public float tilePoint;
    private void Start()
    {
        initialScale = transform.localScale;
        _cycleLength = GameManager.Instance.GetCycleLength();
    }

    public void SetSlot(TileSlot slot,float cycle=0)
    {
        if (cycle == 0)
        {
            cycle = _cycleLength;
        }
        mySlot = slot;
        // transform.position = slot.transform.position; // Snap to slot position
        transform.DOMove(slot.transform.position, cycle).SetEase(Ease.OutSine);
    } 
    public void AssignSlot(TileSlot slot)
    {
        mySlot = slot;
         transform.position = slot.transform.position; // Snap to slot position
        //transform.DOMove(slot.transform.position, _cycleLength).SetEase(Ease.OutSine);
    }

    public void DeleteTile()
    {
        Destroy(gameObject);
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
