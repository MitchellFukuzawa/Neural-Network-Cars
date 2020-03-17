using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveData", menuName = "ScriptableObjects/SaveData", order = 1)]
public class SaveData : ScriptableObject
{
    // want to save between generations the top 6 cars
    // This list will have each cars neural network
    //public GameObject[] Top6Cars;

    // [top car number, N number of weights it has]
    public float[,] weights = new float[6,38];

    public int generation = 0;

    public GameObject finalCar;
}
