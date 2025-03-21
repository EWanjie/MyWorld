using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance;

    [SerializeField]
    private Transform background;

    [System.NonSerialized]
    public float minLim, maxLim;

    private float minX, maxX;
    private Vector2 lastTouchPosition;

    Collider2D draggedObject;
    private bool isMove = false;
    private bool isHold = false;    

    private float moveSpeed = 0.2f;
    private float dragSpeed = 8f;
    private float edgeThreshold = 0.05f;    

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Camera movement boundaries
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();

        minLim = background.GetComponent<SpriteRenderer>().bounds.min.x;
        maxLim = background.GetComponent<SpriteRenderer>().bounds.max.x;

        float backgroundWidth = sr.bounds.size.x;
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;

        minX = background.position.x - (backgroundWidth / 2) + cameraWidth;
        maxX = background.position.x + (backgroundWidth / 2) - cameraWidth;
    }

    void Update()
    {
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame) //First click
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position.ReadValue()), Vector2.zero);   

                if (hit.collider != null && hit.collider.CompareTag("Item")) //If you click on an item
                {
                    draggedObject = hit.collider;
                    isHold = true;
                }
                else
                {
                    lastTouchPosition = touch.position.ReadValue();
                    isMove = true;
                }

            }
            else if (touch.press.isPressed) //Moving
            {
                if (isMove)
                {
                    MoveCamera();
                }
                else if (isHold)
                {
                    MoveCameraWithObject();
                }

            }
            else if (touch.press.wasReleasedThisFrame) //End of click
            {
                isMove = false;
                isHold = false;
            }
        }
    }

    //Camera movement
    private void MoveCamera() 
    {
        Vector2 currentTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector2 delta = currentTouchPosition - lastTouchPosition;

        float moveX = -delta.x * moveSpeed * Time.deltaTime;
        float newX = Mathf.Clamp(transform.position.x + moveX, minX, maxX);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        lastTouchPosition = currentTouchPosition;
    }

    private void MoveCameraWithObject()
    {
        //Object size
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(draggedObject.transform.position);
        float objectWidth = draggedObject.bounds.extents.x;

        //Camera size
        float cameraLeft = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float cameraRight = Camera.main.ViewportToWorldPoint(Vector3.one).x;

        //Limit the object so that it does not go beyond the boundaries of the camera
        float clampedX = Mathf.Clamp(draggedObject.transform.position.x, cameraLeft + objectWidth, cameraRight - objectWidth);
        draggedObject.transform.position = new Vector3(clampedX, draggedObject.transform.position.y, draggedObject.transform.position.z);

        // Ñamera movement with object
        float moveX = dragSpeed * Time.deltaTime;

        if (screenPoint.x <= edgeThreshold && transform.position.x > minX)
        {
            float newX = Mathf.Clamp(transform.position.x - moveX, minX, maxX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else if (screenPoint.x >= 1 - edgeThreshold && transform.position.x < maxX)
        {
            float newX = Mathf.Clamp(transform.position.x + moveX, minX, maxX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}