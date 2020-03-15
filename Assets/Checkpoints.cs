using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public int CheckpointNumber = 0;

    public float DistanceToPrevious
    {
        get;
        set;
    }

    public float AccumulatedDistance
    {
        get;
        set;
    }
}
