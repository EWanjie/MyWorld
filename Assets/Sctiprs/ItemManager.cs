using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemManager : MonoBehaviour
{
    private Vector2 offset;
    private Rigidbody2D rb;
    private bool isHold = false;

    private bool inCollider = false;
    private PolygonCollider2D totalCollider;
    private Collider2D currentCollider;

    private void Start()
    {
        totalCollider = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isHold = true;
                offset = (Vector2)transform.position - mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isHold = false;            

            if (inCollider && !ColliderExit())
            {
                StartGravity();
            }
        }

        if (isHold)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(mousePosition + offset);
        }
    }

    private bool ColliderExit()
    {
        Vector2 minPoint = new Vector2(totalCollider.bounds.center.x, totalCollider.bounds.min.y);

        if (currentCollider.OverlapPoint(minPoint))
            return true;
        else
            return false;            
    }

    private void StopGravity()
    {
        rb.gravityScale = 0.0f;
        rb.linearVelocity = Vector2.zero;
        inCollider = true;
    }

    private void StartGravity()
    {
        rb.gravityScale = 1.0f;
        inCollider = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!inCollider)
        {
            currentCollider = other.GetComponent<Collider2D>();
            if (ColliderExit())
            {
                StopGravity();
            }
        }  
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        StartGravity();
    }
}