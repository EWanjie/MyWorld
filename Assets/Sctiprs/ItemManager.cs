using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    private Vector2 offset;
    private Rigidbody2D rb;
    private bool isHold = false;

    private bool inCollider = false;
    private PolygonCollider2D totalCollider;
    private Collider2D currentCollider;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        totalCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        SetStatePosition(); //Set initial range
    }

    private void Update()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame) // Если палец коснулся экрана
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject) // Если попали в объект
                {
                    SetPriorityPosition();
                    isHold = true;
                }
            }
            else if (touch.press.isPressed && isHold) // Если палец двигается
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                transform.position = new Vector3(touchPosition.x, touchPosition.y, transform.position.z);
            }
            else if (touch.press.wasReleasedThisFrame) // Если палец отпустили
            {
                if (isHold)
                {
                    if (inCollider && !ColliderExit())
                    {
                        StartGravity();
                    }
                    else
                    {
                        SetStatePosition();
                    } 
                }

                isHold = false;
            }
        }
    }

    //Сommon rendering
    private void SetStatePosition()
    {
        spriteRenderer.sortingOrder = 1;

        Vector3 viewPos = transform.position;
        viewPos.z = viewPos.y;
        transform.position = viewPos;   
    }

    //Priority rendering
    private void SetPriorityPosition()
    {
        spriteRenderer.sortingOrder = 100;

        Vector3 viewPos = transform.position;
        viewPos.z = - 9;
        transform.position = viewPos;
    }

    //The object came out of collision along the lower boundary
    private bool ColliderExit()
    {
        Vector2 minPoint = new Vector2(totalCollider.bounds.center.x, totalCollider.bounds.min.y);

        if (currentCollider.OverlapPoint(minPoint))
            return true;
        else
            return false;            
    }

    //Turn off gravity
    private void StopGravity()
    {
        rb.gravityScale = 0.0f;
        rb.linearVelocity = Vector2.zero;
        inCollider = true;

        SetStatePosition();
    }

    //Turn on gravity
    private void StartGravity()
    {
        rb.gravityScale = 1.0f;
        inCollider = false;
    }

    //The object has entered into a collision
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Furniture"))
            return;

        if(!inCollider)
        {
            currentCollider = other.GetComponent<Collider2D>();
            if (ColliderExit() && !isHold)
            {
                StopGravity();
            }
        }  
    }

    //The object is out of collision
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Furniture"))
            return;

        StartGravity();
    }

}