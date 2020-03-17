using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class Manager : MonoBehaviour
{
    private Checkpoints[] checkpoints;
    private float TrackLength;
    public GameObject[] CarsInSimulation;

    // Used to create the cars
    public int carPool;
    public GameObject car;

    public int Generation = 1;

    // List used once the generation has completed to rank
    // cars based on their fitness rating
    public List<GameObject> CarRankings;

    // Another list that stores the best 6 car from the simulation
    public List<GameObject> BestCars;

    private void Awake()
    {
        checkpoints = GetComponentsInChildren<Checkpoints>();
        //CalculateCheckpointPercentages(); // total track length
    }

    private void Start()
    {
        CarsInSimulation = new GameObject[carPool];
        

        for (int i = 0; i < carPool; i++)
        {
            GameObject inst = Instantiate(car);
            inst.transform.position = checkpoints[0].transform.position;

            // Add the car to a list to keep track of all cars
            CarsInSimulation[i] = inst;
        }
    }

    private void Update()
    {
        // if the cars in the scene are all dead the simulation is over
        // When the sim is over we want to rank from best at [0] to worst at [29]
        if (CheckIfCarsAreDead())
        {
            // Sorts the array high to lowest
            bubbleSort(CarsInSimulation);

            for (int i = 0; i < CarsInSimulation.Length; i++)
            {

            }
        }

    }

    // Bubble sort for first place cars
    static void bubbleSort(GameObject[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (arr[j].GetComponent<NeuralNetwork>().fitness < arr[j + 1].GetComponent<NeuralNetwork>().fitness)
                {
                    // swap temp and arr[i] 
                    GameObject temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }


    }

    // returns true if all cars are dead
    private bool CheckIfCarsAreDead()
    {
        
        for (int i = 0; i < CarsInSimulation.Length; i++)
        {
            // if a car is still alive report back
            if (!CarsInSimulation[i].GetComponent<CarController>().isCarDead)
            {
                return false;
            }
        }

        // if all cars are dead say true
        return true;
    }

    private void NextGeneration()
    {
        SceneManager.LoadScene(0);
    }


}
