using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BubbleCursor bubbleCursor;
    private StudyBehavior studyBehavior;

    private void Awake()
    {
        bubbleCursor = FindObjectOfType<BubbleCursor>();
        studyBehavior = FindObjectOfType<StudyBehavior>();
        CSVManager.SetFilePath(studyBehavior.StudySettings.cursorType.ToString());
    }

    private void Start()
    {

        SetCursor(studyBehavior.StudySettings.cursorType);
    }


    public void SetCursor(CursorType cursor)
    {
        switch (cursor)
        {
            case CursorType.PointCursor:
                bubbleCursor.radius = 0.3f;
                break;
            case CursorType.BubbleCursor:
                bubbleCursor.radius = 10f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null);
        }
    }
}