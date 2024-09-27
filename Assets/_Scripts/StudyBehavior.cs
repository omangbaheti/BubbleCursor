using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


[Serializable]
public class TrialConditions
{
    public float amplitude;
    public float targetSize;
    public float EWToW_Ratio;
}

[Serializable]
public class StudySettings
{
    public List<float> targetSizes;
    public List<float> targetAmplitudes;
    public List<float> EWToW_Ratio;
    public CursorType cursorType;
}

public enum CursorType
{
    PointCursor = 0,
    BubbleCursor = 1
}

public class StudyBehavior : MonoBehaviour
{
    public TrialConditions CurrentTrial => blockSequence[currentTrialIndex];
    public StudySettings StudySettings => studySettings;

    public int ParticipantID
    {
        get => participantID;
        set => participantID = value;
    }

    private int participantID;
    [SerializeField] private StudySettings studySettings;
    [SerializeField] private int repetitions;
    [SerializeField] List<TrialConditions> blockSequence = new();

    private float timer = 0f;
    private int misClick = 0;
    private int currentTrialIndex = 0;
    private int missedClicks;

    private string[] header =
    {
        "PID",
        "CT",
        "A",
        "W",
        "EWW",
        "MT",
        "MissedClicks"
    };

    private void Start()
    {
        LogHeader();
        CreateBlock();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void NextTrial()
    {
        LogData();
        currentTrialIndex++;
        if (currentTrialIndex >= blockSequence.Count)
        {
            SceneManager.LoadScene("EndScreen");
        }
    }

    private void CreateBlock()
    {
        for (int i = 0; i < repetitions; i++)
        {
            foreach (float EW in studySettings.EWToW_Ratio)
            {
                foreach (float size in studySettings.targetSizes)
                {
                    foreach (float amp in studySettings.targetAmplitudes)
                    {

                        blockSequence.Add(new TrialConditions()
                        {
                            amplitude = amp,
                            targetSize = size,
                            EWToW_Ratio = EW,
                        });
                    }
                }
            }
        }
        blockSequence = YatesShuffle(blockSequence);
    }

    private void LogHeader()
    {
        CSVManager.AppendToCSV(header);
    }

    private void LogData()
    {
        string[] data =
        {
            participantID.ToString(),
            studySettings.cursorType.ToString(),
            blockSequence[currentTrialIndex].amplitude.ToString(),
            blockSequence[currentTrialIndex].targetSize.ToString(),
            blockSequence[currentTrialIndex].EWToW_Ratio.ToString(),
            timer.ToString(),
            misClick.ToString()
        };
        CSVManager.AppendToCSV(data);
        timer = 0f;
        misClick = 0;
    }

    public void HandleMisClick()
    {
        misClick++;
    }

    public void SetParticipantID(int ID)
    {
        participantID = ID;
    }

    private static List<T> YatesShuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }
}


