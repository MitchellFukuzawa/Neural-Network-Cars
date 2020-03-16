using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Manager : MonoBehaviour
{
    private Checkpoints[] checkpoints;
    private float TrackLength;

    public int carPool = 10;
    public GameObject car;

    private void Awake()
    {
        checkpoints = GetComponentsInChildren<Checkpoints>();
        //CalculateCheckpointPercentages(); // total track length
    }

    private void Start()
    {
        for (int i = 0; i < carPool; i++)
        {
            GameObject inst = Instantiate(car);
            inst.transform.position = checkpoints[0].transform.position;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartSimulation();
    }

    private void RestartSimulation()
    {
        SceneManager.LoadScene(0);
    }

    //private void CalculateCheckpointPercentages()
    //{
    //    checkpoints[0].AccumulatedDistance = 0;
    //    for (int i = 1; i < checkpoints.Length; i++)
    //    {
    //        checkpoints[i].DistanceToPrevious = Vector2.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
    //        checkpoints[i].AccumulatedDistance = checkpoints[i - 1].AccumulatedDistance + checkpoints[i].DistanceToPrevious;
    //    }
    //    TrackLength = checkpoints[checkpoints.Length - 1].AccumulatedDistance;
    //}
}
