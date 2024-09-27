using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private BubbleCursor bubbleCursor;
    private StudyBehavior studyBehavior;
    private int participantID;
    private void Awake()
    {
        bubbleCursor = FindObjectOfType<BubbleCursor>();
        studyBehavior = FindObjectOfType<StudyBehavior>();
        CSVManager.SetFilePath(studyBehavior.StudySettings.cursorType.ToString());
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SetCursor(studyBehavior.StudySettings.cursorType);
    }

    public void SetCursor(CursorType cursor)
    {
        bubbleCursor.radius = cursor switch
        {
            CursorType.PointCursor => 0.015f,
            CursorType.BubbleCursor => 10f,
            _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null)
        };
    }

    public void StartStudy()
    {
        if(inputField.text == string.Empty) return;
        participantID = int.Parse(inputField.text);
        studyBehavior.ParticipantID = participantID;
    }
}