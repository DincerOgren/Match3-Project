using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector3 initialScale;
    public ObjectType objectType;


    private void Start()
    {
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

