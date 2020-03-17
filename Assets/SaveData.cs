using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveData", menuName = "ScriptableObjects/SaveData", order = 1)]
public class SaveData : ScriptableObject
{
    // want to save between generations the top 6 cars
    // This list will have each cars neural network
    public GameObject[] Top6Cars;

    public int previousGeneration;

    public GameObject finalCar;
}
