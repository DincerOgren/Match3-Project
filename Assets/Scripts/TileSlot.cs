using System.Collections;
using UnityEngine;


public class TileSlot : MonoBehaviour
{
    //TODO: Privatte name convencion;
    public Tile currentTile;

    [SerializeField] GameObject destroyFX;

    public Vector2Int tileIndex;


    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    [SerializeField] float swipeThreshold = 0.5f;
    bool validTouch = false;
    bool validTouchEnd = false;

    private void OnMouseDown()
    {

        // DISABLE HERE FOR MOBILE INPUT TO WORK

        if (GameManager.Instance.CanSwap())
        {

            GameManager.Instance.SelectTile(currentTile);
        }

    }

    private void Update()
    {
        HandleTouchInput();
    }
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (GameManager.Instance.CanSwap() && IsTouchingTile(touchPos))
                    {
                        validTouch = true;
                        touchStartPos = touchPos;
                        print("Touch start pos =" + touchStartPos);
                    }
                    else
                        validTouch = false;
                    break;

                case TouchPhase.Ended:
                    touchEndPos = touchPos;
                    Vector2 swipeDirection = touchEndPos - touchStartPos;


                    if (swipeDirection.magnitude >= swipeThreshold && validTouch)
                    {

                        print("touch sucsessfull");
                        print("Touch end pos = " + touchEndPos);
                        print("Touch swiper dir = " + swipeDirection);
                        print("Touch swiper dir mag = " + swipeDirection.magnitude);
                        ProcessSwipe(swipeDirection);
                    }
                    else if (validTouch)
                    {
                        print("Touch unsucsessful");
                        print("Touch end pos = " + touchEndPos);
                        print("Touch swiper dir = " + swipeDirection);
                        print("Touch swiper dir mag = " + swipeDirection.magnitude);
                    }
                    break;
            }
        }
    }

    private bool IsTouchingTile(Vector2 touchPos)
    {
        // Check if the touch is within the tile's bounds
        return GetComponent<Collider2D>().OverlapPoint(touchPos);
    }

    private void ProcessSwipe(Vector2 swipeDirection)
    {
        // Determine the dominant direction (horizontal or vertical)
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y)) // Horizontal swipe
        {
            print("Horiz swipe");
            if (swipeDirection.x > 0)
                AttemptSwap(Vector2Int.right); // Swipe Right
            else
                AttemptSwap(Vector2Int.left);  // Swipe Left
        }
        else // Vertical swipe
        {
            print("Vertical swipe");

            if (swipeDirection.y > 0)
                AttemptSwap(Vector2Int.up);    // Swipe Up
            else
                AttemptSwap(Vector2Int.down);  // Swipe Down
        }
    }

    private void AttemptSwap(Vector2Int direction)
    {
        Vector2Int tileSize = GameManager.Instance.GetGridSize();
        GameManager.Instance.SelectTile(currentTile); // Select first tile

        int x = direction.x + tileIndex.x;
        int y = direction.y + tileIndex.y;

        if (x < 0 || x > tileSize.x)
        {
            print("invalid hor swipe ");
            return;
        }
        if (y < 0 || y > tileSize.y)
        {
            print("invalid ver swipe");
            return;
        }
        print("Second tile x = " + x + " y = " + y);

        GameManager.Instance.SelectTile(GameManager.Instance.GetTileSlot(x, y).GetTile()); // Select second tile and trigger swap


        //if (direction == Vector2.down && tileIndex.y != 0)
        //{

        //}

        //if (direction == Vector2.up && tileIndex.y != tileSize.y)
        //{

        //}  

        //if (direction == Vector2.down && tileIndex.x != tileSize.x)
        //{

        //}

        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f);
        //if (hit.collider != null && hit.collider.GetComponent<Tile>())
        //{
        //    Tile otherTile = hit.collider.GetComponent<Tile>();
        //    print("attmept swap");

        //}
    }



    public void SetTile(Tile tile, bool shouldSpawnInstant = false, float speed = 0)
    {
        currentTile = tile;
        tile.index = tileIndex;
        if (shouldSpawnInstant)
        {
            tile.AssignSlot(this);
        }
        else
            tile.SetSlot(this, speed); // Let the tile know its slot
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
        var fx = Instantiate(destroyFX, transform);
        fx.transform.position = transform.position;


        currentTile.DeleteTile();

        ClearTile();
    }
    public Tile GetTile() => currentTile;
}
