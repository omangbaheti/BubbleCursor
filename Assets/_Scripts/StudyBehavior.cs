using System;
using System.Collections.Generic;
using UnityEngine;
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

    public int participantID;
    [SerializeField] private StudySettings studySettings;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private List<float> EWToW_Ratio;
    [SerializeField] private CursorType cursorType;
    [SerializeField] private int repetitions;
    [SerializeField] private int randomTargetsNumber = 20;
    [SerializeField] List<TrialConditions> blockSequence = new();

    private int misclick;
    private float timer = 0f;
    private int misClick = 0;
    private int currentTrialIndex;
    private int missedClicks;
    private int cursorTypeIndex = 0;

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
        if (currentTrialIndex == blockSequence.Count - 1)
        {
            Application.Quit();
        }
    }

    private void CreateBlock()
    {
        for (int i = 0; i < repetitions; i++)
        {
            foreach (float EW in EWToW_Ratio)
            {
                foreach (float size in targetSizes)
                {
                    foreach (float amp in targetAmplitudes)
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
            cursorType.ToString(),
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


