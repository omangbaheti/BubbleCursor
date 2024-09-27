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

    [SerializeField] private int randomTargetsNumber = 20;
    private Vector3 ScreenCentreInWorld => mainCamera.ScreenToWorldPoint(screenCentre) + new Vector3(0,0,10);
    private List<float> randomSizes;
    private Camera mainCamera;
    private GameManager gameManager;
    private Vector2 screenCentre;
    private Vector3 currentSpawnPosition;
    private StudyBehavior studyBehavior;

    private void Awake()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        screenCentre = new Vector2(Screen.width/2, Screen.height / 2);
    }

    private void Start()
    {
        SpawnScreenCentreTarget();
    }

    #region PublicFacingCoreLogic

    public void SpawnNextTarget(bool reset)
    {
        StartCoroutine(NextTarget(reset));
    }

    public IEnumerator NextTarget(bool reset)
    {
        yield return new WaitForSeconds(0.1f);
        //Clear all targets before spawning the next one
        Target[] targets = FindObjectsByType<Target>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Target target in targets)
        {
            Destroy(target.gameObject);
        }

        //If a target is selected, spawn "reset" target at screen centre
        if (reset)
        {
            gameManager.SetCursor(CursorType.PointCursor);
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
        Vector3 spawnOffset = randomAngleDisplacement * new Vector3(studyBehavior.CurrentTrial.amplitude, 0f, 0f);
        currentSpawnPosition = ScreenCentreInWorld + spawnOffset;
        GameObject targetObject = Instantiate(target, currentSpawnPosition, Quaternion.identity, transform);
        targetObject.transform.localScale = studyBehavior.CurrentTrial.targetSize * Vector3.one;
        targetObject.GetComponent<Target>().SetMainTarget();
        SpawnDistractorTargets(currentSpawnPosition);
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
        float effectiveWidth = studyBehavior.CurrentTrial.EWToW_Ratio * studyBehavior.CurrentTrial.targetSize;

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
            targetObject.transform.localScale = studyBehavior.CurrentTrial.targetSize * Vector3.one;
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
            int randomIndex = Random.Range(0, studyBehavior.StudySettings.targetSizes.Count);
            sizes.Add(studyBehavior.StudySettings.targetSizes[randomIndex]);
        }

        return sizes;
    }

    private bool CheckPositionValidity(Vector3 _pos)
    {
        float effectiveWidth = studyBehavior.CurrentTrial.EWToW_Ratio * studyBehavior.CurrentTrial.targetSize;
        return Vector3.Distance(_pos, currentSpawnPosition) < effectiveWidth;
    }

    #endregion


}

