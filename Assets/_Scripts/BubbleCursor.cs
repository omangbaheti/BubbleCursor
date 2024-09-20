using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleCursor : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private ContactFilter2D contactFilter;
    [SerializeField] private Transform bubbleVisual;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private Collider2D previousDetectedCollider = new();

    private void Awake()
    {
        mainCam = Camera.main;
        bubbleVisual.localScale = Vector2.zero;
    }

    void Update()
    {

        float minDistance = Mathf.Infinity;
        Collider2D detectedCollider = null;
        Physics2D.OverlapCircle(transform.position, radius, contactFilter, results);

        foreach (Collider2D col in results)
        {
            Vector2 flattenedTransform = new(transform.position.x, transform.position.y);
            Vector2 flattenedColliderPosition = new(col.transform.position.x, col.transform.position.y);
            float distance = (flattenedColliderPosition - flattenedTransform).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                detectedCollider = col;
            }
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z += 10f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        if (detectedCollider == null)
        {

            return;
        }

        ExpandBubble(detectedCollider, minDistance);

        if (previousDetectedCollider != null && detectedCollider != previousDetectedCollider)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget(detectedCollider);
        }

        previousDetectedCollider = detectedCollider;
    }

    private void ExpandBubble(Collider2D collider, float expandedBubbleRadius)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnHoverEnter();
            bubbleVisual.transform.DOScale(expandedBubbleRadius*2+1f, 0.1f);
        }
        else
        {
            Debug.LogWarning("Not a valid Target?");
        }
    }

    void SelectTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnSelect();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
