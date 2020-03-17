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
        saveData.generation++;
        CarsInSimulation = new GameObject[carPool];

        if (saveData.generation == 1)
        {
            // Clean up previous data
            //saveData.Top6Cars = new GameObject[6];


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
        if(saveData.generation > 1)
        {
            // Spawn in top 6 cars
            //for (int i = 0; i < 6; i++)

            //print("Debug_TopCars: " + saveData.Top6Cars.Length);

            // For all top 6 spawned in cars
            for (int c = 0; c < 6; c++)
            {
                GameObject inst = Instantiate(car);
                CarsInSimulation[c] = inst;

                inst.GetComponent<NeuralNetwork>().initializeLayers();

                // for all layers but the input layer
                for (int n = 1; n < CarsInSimulation[c].GetComponent<NeuralNetwork>().layers.Count; n++)
                {
                    // for all neurons
                    for (int j = 0; j < CarsInSimulation[c].GetComponent<NeuralNetwork>().layers[n].neurons.Count; j++)
                    {
                        // for all weights
                        for (int k = 0; k < CarsInSimulation[c].GetComponent<NeuralNetwork>().topology[n - 1]; k++)
                        {
                            // Send the data into the scriptable object array
                            //saveData.weights[i, k] = CarsInSimulation[c].GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights[k];

                            inst.GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights.Add(saveData.weights[c, k + j]);
                        }
                    }
                }
            }

            for (int c = 6; c < 30; c++)
            {
                GameObject inst = Instantiate(car);
                CarsInSimulation[c] = inst;

                inst.GetComponent<NeuralNetwork>().initializeLayers();
                // for all layers
                for (int n = 1; n < CarsInSimulation[c].GetComponent<NeuralNetwork>().topology.Length; n++)
                {
                    // for all neurons
                    for (int j = 0; j < CarsInSimulation[c].GetComponent<NeuralNetwork>().topology[n]; j++)
                    {
                        // for all weights
                        for (int k = 0; k < CarsInSimulation[c].GetComponent<NeuralNetwork>().topology[n-1]; k++)
                        {
                            // Send the data into the scriptable object array
                            //saveData.weights[i, k] = CarsInSimulation[c].GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights[k];

                            //inst.GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights[k] = saveData.weights[c, k + j + n];
                            print("k j n: " + k + j + n);

                            print("Neurons: " + inst.GetComponent<NeuralNetwork>().layers[n].neurons.Count);
                            inst.GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights.Add(saveData.weights[c, k + j]);
                        }
                    }
                }

                inst.GetComponent<NeuralNetwork>().CreateNewCar(CarsInSimulation[0].GetComponent<NeuralNetwork>(), CarsInSimulation[1].GetComponent<NeuralNetwork>(), inst.GetComponent<NeuralNetwork>());
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
            NextGeneration();
        }

        // debug
        if (Input.GetKeyDown(KeyCode.D))
        {
            for (int i = 0; i < saveData.weights.GetLength(1); i++)
            {
                print("Best Car: " + saveData.weights[0,i]);
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
        // Saves the weights of all top 6 cars in saveData.weights[,]
        // For top 6 cars
        for (int i = 0; i < 6; i++)
        {
            // for all layers
            for (int n = 0; n < CarsInSimulation[i].GetComponent<NeuralNetwork>().layers.Count; n++)
            {
                // for all neurons
                for (int j = 0; j < CarsInSimulation[i].GetComponent<NeuralNetwork>().layers[n].neurons.Count; j++)
                {
                    // for all weights
                    for (int k = 0; k < CarsInSimulation[i].GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights.Count; k++)
                    {
                        // Send the data into the scriptable object array
                        saveData.weights[i,k] = CarsInSimulation[i].GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights[k];

                    }
                }
            }
        }
        print("Best Fitness for gen: " + saveData.generation + "     Fitness: " + CarsInSimulation[0].GetComponent<NeuralNetwork>().fitness);
        print("Best Avg Speed: " + CarsInSimulation[0].GetComponent<CarController>().CalculateAvgSpeed(CarsInSimulation[0].GetComponent<CarController>().SpeedDuringSimulation));
        SceneManager.LoadScene(0);
    }

    private void OnApplicationQuit()
    {
        print("-----------------------------------");
        saveData.generation = 0;
        saveData.weights = new float[6,38];
    }
}
