using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;


public class Manager : MonoBehaviour
{
    private Checkpoints[] checkpoints;
    private float TrackLength;
    public GameObject[] CarsInSimulation;
    public Transform spawnPosition;

    // Used to create the cars
    public int carPool;
    public GameObject car;
    // [top car number, N number of weights it has]
    public float[,] weights = new float[6, 38];
    public int Generation = 1;
    public SaveData saveData;

    // List used once the generation has completed to rank
    // cars based on their fitness rating
    public List<GameObject> CarRankings;

    // Another list that stores the best 6 car from the simulation
    public List<GameObject> BestCars;

    public static Manager _instance;
    private bool hasManagerInitialized = false;

    private void Awake()
    {
        checkpoints = GetComponentsInChildren<Checkpoints>();
        //CalculateCheckpointPercentages(); // total track length

        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }


    public void OnSceneLoaded(Scene s, LoadSceneMode l)
    {

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
            // For all top 6 spawned in cars
            for (int c = 0; c < 6; c++)
            {
                GameObject inst = Instantiate(car);
                CarsInSimulation[c] = inst;
                inst.name = "car " + c;
                inst.GetComponent<NeuralNetwork>().initializeLayers();
                inst.transform.position = checkpoints[0].transform.position;
                int[] mathArray = CarsInSimulation[c].GetComponent<NeuralNetwork>().topology;
                int numberOfAddedWeights = 0;

                // Setting each top car from previous gen their weights back
                // for all layers but the input layer
                for (int n = 1; n < mathArray.Length; n++)
                {
                    // for all neurons
                    for (int j = 0; j < mathArray[n]; j++)
                    {
                        // for all weights
                        for (int k = 0; k < mathArray[n - 1]; k++)
                        {
                            // Send the data into the scriptable object array
                            inst.GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights.Add(weights[c, numberOfAddedWeights]);
                            numberOfAddedWeights++;
                        }
                    }
                }
            }

            // Spawn in the rest of the 24 cars and use on them-> Genetic algorithm function "CreateNewCar"
            for (int c = 6; c < 30; c++)
            {
                GameObject inst = Instantiate(car);
                CarsInSimulation[c] = inst;
                inst.name = "car " + c;
                inst.transform.position = checkpoints[0].transform.position;

                inst.GetComponent<NeuralNetwork>().initializeLayers();
                // for all layers
                int[] mathArray = CarsInSimulation[c].GetComponent<NeuralNetwork>().topology;
                int numberOfAddedWeights = 0;

                // for all layers but the input layer
                for (int n = 1; n < mathArray.Length; n++)
                {
                    // for all neurons
                    for (int j = 0; j < mathArray[n]; j++)
                    {
                        // for all weights
                        for (int k = 0; k < mathArray[n - 1]; k++)
                        {
                            // Send the data into the scriptable object array
                            inst.GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights.Add(weights[0, numberOfAddedWeights]);
                            numberOfAddedWeights++;
                        }
                    }
                }

                // Uses top 2 car from previous generation to create a baby
                // randomly picks which weight to use from best and second best parents
                // then has a chance to mutate
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
            for (int i = 0; i < weights.GetLength(1); i++)
            {
                print("Best Car: " + weights[0,i]);
            }
        }

        // Saving weights
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save_BestFitnessWeights();
        }

        // Load weights
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load_FitnessWeights();
        }
    }

    // Bubble sort for first place cars
    static void bubbleSort(GameObject[] a)
    {
        int n = a.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (a[j].GetComponent<NeuralNetwork>().fitness < a[j + 1].GetComponent<NeuralNetwork>().fitness)
                {
                    // swap temp and arr[i] 
                    GameObject temp = a[j];
                    a[j] = a[j + 1];
                    a[j + 1] = temp;
                }
    }

    // returns true if all cars are dead
    private bool CheckIfCarsAreDead()
    {        
        for (int i = 0; i < CarsInSimulation.Length; i++)
        {
            // if a car is still alive report back
            if (CarsInSimulation[i] && !CarsInSimulation[i].GetComponent<CarController>().isCarDead)
            {
                return false;
            }
        }

        // if all cars are dead say true
        return true;
    }

    // Where the data is saved
    private void NextGeneration()
    {
        // Saves the weights of all top 6 cars in saveData.weights[,]
        // For top 6 cars
       
        for (int i = 0; i < 6; i++)
        {
            int weightCount = 0;
            // for all layers
            for (int n = 1; n < CarsInSimulation[i].GetComponent<NeuralNetwork>().topology.Length; n++)
            {
                // for all neurons
                for (int j = 0; j < CarsInSimulation[i].GetComponent<NeuralNetwork>().topology[n]; j++)
                {
                    // for all weights
                    for (int k = 0; k < CarsInSimulation[i].GetComponent<NeuralNetwork>().topology[n - 1]; k++)
                    {
                        // Send the data into the Manager to carry over on restart
                        weights[i, weightCount] = CarsInSimulation[i].GetComponent<NeuralNetwork>().layers[n].neurons[j].incomingWeights[k];
                        weightCount++;
                    }
                }
            }
        }
        print("Best Fitness for gen: " + saveData.generation + "     Fitness: " + CarsInSimulation[0].GetComponent<NeuralNetwork>().fitness);
        print("Best Avg Speed: " + CarsInSimulation[0].GetComponent<CarController>().CalculateAvgSpeed(CarsInSimulation[0].GetComponent<CarController>().SpeedDuringSimulation));

        hasManagerInitialized = false;
        saveData.generation++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Clean up
    private void OnApplicationQuit()
    {
        print("-----------------------------------");
        saveData.generation = 1;
        weights = new float[6,38];
    }

    // Connected to a button on the UI
    // When pressed loops through weight[,] storage array
    public void Save_BestFitnessWeights()
    {


        print("___________________hi");
        string serializedData = "";

        for (int i = 0; i < 38; i++)
        {
            print("Weight Data 0:" + weights[0, i]);
            serializedData += "Weight " + i + ":" + weights[0, i] + "\n";
        }
        print("DATA:" + serializedData);
        StreamWriter writer = new StreamWriter("weight.txt", true);
        writer.Write(serializedData);

    }

    // When loading we read the weights, set and then load next generation
    public void Load_FitnessWeights()
    {
        int i = 0;
        StreamReader reader = new StreamReader("weight.txt");
        while (!reader.EndOfStream)
        {
            i++;
            string lineA = reader.ReadLine();
            string[] splitA = lineA.Split(':');
            float weightvalue = float.Parse(splitA[1]);
            print("Line:" + i + "      Weight:" + weightvalue);

            weights[0, i] = weightvalue;
            weights[1, i] = weightvalue;
        }

        NextGeneration();
    }
}
