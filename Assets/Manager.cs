using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private Checkpoints[] checkpoints;
    private float TrackLength;

    private void Awake()
    {
        checkpoints = GetComponentsInChildren<Checkpoints>();
        //CalculateCheckpointPercentages(); // total track length
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
