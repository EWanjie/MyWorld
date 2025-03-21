using UnityEngine;
using static MoveManager;

public class ItemLimetManager : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        //Object movement boundaries
        mainCamera = Camera.main;

        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); //Object boundaries
        objectWidth = spriteRenderer.bounds.extents.x;
        objectHeight = spriteRenderer.bounds.extents.y;
    }

    void LateUpdate() 
    {

        //Restriction of object movement
        Vector3 viewPos = transform.position;

        viewPos.x = Mathf.Clamp(viewPos.x, MoveManager.Instance.minLim + objectWidth, MoveManager.Instance.maxLim - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);

        transform.position = viewPos;
    }
}
