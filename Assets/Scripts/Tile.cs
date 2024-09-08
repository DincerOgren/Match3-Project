using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        
    }

    private void OnMouseDown()
    {
        GameManager.Instance.SelectTile(this);
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

