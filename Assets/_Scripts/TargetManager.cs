using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetManager : MonoBehaviour
{
    public int participantID;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject resetTarget;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private List<float> EWToW_Ratio;
    [SerializeField] private int repetitions;
    [SerializeField] private int randomTargetsNumber = 20;
    [SerializeField] List<TrialConditions> blockSequence = new();
    private Vector3 ScreenCentreInWorld => mainCamera.ScreenToWorldPoint(screenCentre) + new Vector3(0,0,10);
    private List<float> randomSizes;
    private Camera mainCamera;
    private List<Target> targetList = new();
    private Vector2 screenCentre;
    private int currentTrialIndex;
    private Vector3 currentSpawnPosition;
    private float timer = 0f;
    private void Start()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width/2, Screen.height / 2);
        CreateBlock();
        SpawnScreenCentreTarget();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    private void CreateBlock()
    {
        //Iterating through each EW, targetSize and amplitude and building every combination
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
                            EWToW_Ratio = EW
                        });
                    }
                }
            }
        }

        blockSequence = YatesShuffle(blockSequence);

        // foreach (TrialConditions var in blockSequence)
        // {
        //     Debug.Log($"{var.amplitude} {var.EWToW_Ratio} {var.targetSize}");
        // }
    }

    #region PublicFacingCoreLogic

    public void SpawnNextTarget(bool reset)
    {
        StartCoroutine(NextTarget(reset));
    }

    public IEnumerator NextTarget(bool reset)
    {
        LogData();
        yield return new WaitForSeconds(0.1f);
        //Clear all targets before spawning the next one
        timer = 0f;
        Target[] targets = FindObjectsByType<Target>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Target target in targets)
        {
            Destroy(target.gameObject);
        }

        //If the a target is selected, spawn reset target
        if (reset)
        {
            SpawnScreenCentreTarget();
        }
        else // if the "reset" target is selected, spawn the next target in the block
        {
            SpawnMainTarget();
            SpawnRandomTargets();
        }
    }

    #endregion

    #region Main Target Spawn Logic

    private void SpawnMainTarget()
    {
        //Calculate Spawn position
        float randomAngle = Random.Range(0, 360);
        Quaternion randomAngleDisplacement = Quaternion.Euler(0,0,randomAngle);
        Vector3 spawnOffset = randomAngleDisplacement * new Vector3(blockSequence[currentTrialIndex].amplitude, 0f, 0f);
        currentSpawnPosition = ScreenCentreInWorld + spawnOffset;
        GameObject targetObject = Instantiate(target, currentSpawnPosition, Quaternion.identity, transform);
        targetObject.transform.localScale = blockSequence[currentTrialIndex].targetSize * Vector3.one;
        targetObject.GetComponent<Target>().Highlight();
        SpawnDistractorTargets(currentSpawnPosition);
        currentTrialIndex++;
    }

    private void SpawnScreenCentreTarget()
    {
        GameObject targetObject = Instantiate(resetTarget, ScreenCentreInWorld, Quaternion.identity, transform);
        targetObject.transform.localScale = Vector3.one * 0.5f;
    }

    private void SpawnDistractorTargets(Vector3 spawnPosition)
    {
        //Since EWToW_Ratio = EW/W
        //EW = EWToW_Ratio * W
        float effectiveWidth = blockSequence[currentTrialIndex].EWToW_Ratio * blockSequence[currentTrialIndex].targetSize;

        Vector3 centreToTargetVector = spawnPosition - ScreenCentreInWorld;
        Vector3 centreToTargetPerpendicularVector = Vector3.Cross(centreToTargetVector, Vector3.forward);

        List<Vector3> spawnPositions = new()
        {
            //We need to spawn two distractors along this vector at a distance of effectiveWidth
            spawnPosition + centreToTargetVector.normalized * effectiveWidth,
            spawnPosition - centreToTargetVector.normalized * effectiveWidth,
            //We need to spawn two distractors perpendicular to this vector at a distance of effectiveWidth
            spawnPosition + centreToTargetPerpendicularVector.normalized * effectiveWidth,
            spawnPosition - centreToTargetPerpendicularVector.normalized * effectiveWidth
        };

        foreach (Vector3 pos in spawnPositions)
        {
            GameObject targetObject = Instantiate(target, pos, Quaternion.identity, transform);
            targetObject.transform.localScale = blockSequence[currentTrialIndex].targetSize * Vector3.one;
        }
    }

    #endregion

    #region Random Target Spawn Logic

    private void SpawnRandomTargets()
    {
        List<float> randomSizes = GenerateRandomSizes(randomTargetsNumber);
        List<Vector3> points = GenerateRandomPoints(randomTargetsNumber);
        for (int i = 0; i < randomTargetsNumber; i++)
        {
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];
        }
    }

    List<Vector3> GenerateRandomPoints(int numberOfPoints)
    {
        List<Vector3> pointList = new();
        for (int i = 0; i < numberOfPoints; i++)
        {
            bool isValidPosition = false;
            Vector3 randomWorldPoint;

            do
            {
                float randomX = Random.Range(0, Screen.width);
                float randomY = Random.Range(0, Screen.height);
                float z = 10f;
                Vector3 randomScreenPoint = new(randomX, randomY, z);
                randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);
                isValidPosition = CheckPositionValidity(randomWorldPoint);
            }
            while (isValidPosition);

            pointList.Add(randomWorldPoint);
        }
        return pointList;
    }

    List<float> GenerateRandomSizes(int numberOfPoints)
    {
        List<float> sizes = new();
        for (int i = 0; i < numberOfPoints; i++)
        {
            int randomIndex = Random.Range(0, targetSizes.Count);
            sizes.Add(targetSizes[randomIndex]);
        }

        return sizes;
    }

    private bool CheckPositionValidity(Vector3 _pos)
    {
        float effectiveWidth = blockSequence[currentTrialIndex].EWToW_Ratio * blockSequence[currentTrialIndex].targetSize;
        return Vector3.Distance(_pos, currentSpawnPosition) < effectiveWidth;
    }

    #endregion

    private void LogData()
    {
        string[] data =
        {
            participantID.ToString(),
            blockSequence[currentTrialIndex].amplitude.ToString(),
            blockSequence[currentTrialIndex].targetSize.ToString(),
            blockSequence[currentTrialIndex].EWToW_Ratio.ToString(),
            timer.ToString()
        };
        CSVManager.AppendToCSV(data);
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

[Serializable]
public class TrialConditions
{
    public float amplitude;
    public float targetSize;
    public float EWToW_Ratio;
}

