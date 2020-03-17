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
    public SaveData saveData;

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
        if (Generation == 1)
        {
            // Clean up previous data
            saveData.previousGeneration = 1;
            saveData.Top6Cars = new GameObject[6];


            for (int i = 0; i < carPool; i++)
            {
                GameObject inst = Instantiate(car);
                inst.transform.position = checkpoints[0].transform.position;

                // Add the car to a list to keep track of all cars
                CarsInSimulation[i] = inst;
            }
        }
        // Now that the generation has gotten the first results
        // we want to spawn in the top 6
        // and spawn in 24 new cars that are genetically modified
        if(Generation > 1)
        {
            // Spawn in top 6 cars
            for (int i = 0; i < 6; i++)
            {
                GameObject inst = Instantiate(saveData.Top6Cars[i]);
                CarsInSimulation[i] = inst;
            }

            for (int i = 0; i < 24; i++)
            {
                // Create 24 new cars based off of weights of the top 2 cars
                GameObject inst = Instantiate(car);

                // loop through each layer, each neuron and each weight
                inst.GetComponent<NeuralNetwork>().CreateNewCar(saveData.Top6Cars[0].GetComponent<NeuralNetwork>(), saveData.Top6Cars[1].GetComponent<NeuralNetwork>(), inst.GetComponent<NeuralNetwork>());
            }
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

            // take the best 6 cars and store them in the saveData
            for (int i = 0; i < 6; i++)
            {
                saveData.Top6Cars[i] = CarsInSimulation[i];
            }

            NextGeneration();
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
        saveData.previousGeneration++;
        SceneManager.LoadScene(0);
    }


}
