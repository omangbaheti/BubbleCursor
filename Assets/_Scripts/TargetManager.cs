using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private int repetitions;

    private List<float> randomSizes;
    private List<Target> targetList = new();
    private Vector2 screenCentre;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width/2, Screen.height / 2);
        SpawnTargets();

    }

    private void SpawnTargets()
    {
        List<Vector3> points = GenerateRandomPoints();
        List<float> randomSizes = GenerateRandomSizes();
        for (int i = 0; i < repetitions; i++)
        {
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];
        }
    }

    List<Vector3> GenerateRandomPoints()
    {
        List<Vector3> pointList = new();
        for (int i = 0; i < repetitions; i++)
        {
            float randomX = Random.Range(0, Screen.width);
            float randomY = Random.Range(0, Screen.height);
            float z = 10f;
            Vector3 randomScreenPoint = new(randomX, randomY, z);
            Vector3 randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);
            pointList.Add(randomWorldPoint);
        }
        return pointList;
    }

    List<float> GenerateRandomSizes()
    {
        List<float> sizes = new();
        for (int i = 0; i < repetitions; i++)
        {
            int randomIndex = Random.Range(0, targetSizes.Count);
            sizes.Add(targetSizes[randomIndex]);
        }

        return sizes;
    }
}
