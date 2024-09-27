using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int cursorType;
    
    private BubbleCursor bubbleCursor;

    private void Start()
    {
        bubbleCursor = FindObjectOfType<BubbleCursor>();
    }

    public void SetToPointCursor()
    {
        bubbleCursor.radius = 0.05f;
    }
    
    public void SetCursor(CursorType cursor)
    {
        switch (cursor)
        {
            case CursorType.PointCursor:
                bubbleCursor.radius = 0.05f;
                break;
            case CursorType.BubbleCursor:
                bubbleCursor.radius = 10f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null);
        }
    }
}