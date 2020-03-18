using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public List<Layer> layers;
    [SerializeField]
    public int[] topology = new int[] { 5, 4, 3, 2 };
    public Manager manager;
    public CarController carController;

    //[HideInInspector]
    public float fitness = 0;


    public void Start()
    {
        manager = FindObjectOfType<Manager>();
        carController = GetComponent<CarController>();

        

        // Set random weights for only the first generation
        if(manager.saveData.generation == 1)
        {
            initializeLayers();
            SetRandomWeights();

        }
    }

    // Uses top 2 car from previous generation to create a baby
    // randomly picks which weight to use from best and second best parents
    // then has a chance to mutate
    public NeuralNetwork CreateNewCar(NeuralNetwork a, NeuralNetwork b, NeuralNetwork baby)
    {
        //baby.manager = FindObjectOfType<Manager>();
        //baby.initializeLayers();
        for (int i = 0; i < a.layers.Count; i++)
        {
            for (int j = 0; j  < a.layers[i].neurons.Count; j ++)
            {
                for (int k = 0; k < a.layers[i].neurons[j].incomingWeights.Count; k++)
                {
                    int randInt = Random.Range(0, 2);

                    // Inherit parents weights
                    if (randInt > 0)
                        baby.layers[i].neurons[j].incomingWeights[k] = a.layers[i].neurons[j].incomingWeights[k]; // A parent
                    else
                        baby.layers[i].neurons[j].incomingWeights[k] = b.layers[i].neurons[j].incomingWeights[k]; // B parent

                    // Mutation chance per weight
                    float randFloat = Random.value;

                    // 20% chance to mutate the parent weight
                    if(randFloat <= .2f)
                    {
                        baby.layers[i].neurons[j].incomingWeights[k] += .1f;
                    }
                }
            }
        }
        return baby;
    }

    public float[,] GeneticCrossoverMutation(float[,] a, float[,] b, int carNumber)
    {
        float[,] baby = new float[6,38];

        for (int i = 0; i < baby.GetLength(1); i++)
        {
            int randInt = Random.Range(0, 2);

            // Inherit parents weights
            if (randInt > 0)
                baby[carNumber, i] = a[carNumber, i];
            else
                baby[carNumber, i] = b[carNumber, i];

            // Mutation chance per weight
            float randFloat = Random.value;

            // 20% chance to mutate the parent weight
            if (randFloat <= .2f)
            {
                baby[carNumber, i] += 1f;
            }
        }

        return baby;
    }

    // Goes through ever layer to calculate evaluations for the entire network
    public Vector2 ProccessingEvaluations(float[] inputs)
    {

        // Input layer
        for (int i = 0; i < topology[0]; i++)
        {

            layers[0].neurons[i].Evaluation = inputs[i];
        }

        // Hidden layer 1: calculate evaluation for each neuron
        for (int i = 0; i < topology[1]; i++)
        {
            float summation = 0;

            for (int j = 0; j < topology[0]; j++)
            {
                //print("weights: " + layers[1].neurons[i].incomingWeights.Count);

                //             (Preivous layers' evals)         (get current layers incoming weights)

                summation += layers[0].neurons[j].Evaluation * layers[1].neurons[i].incomingWeights[j];
            }


            layers[1].neurons[i].Evaluation = Activation(summation) - .5f;
        }

        // Hidden layer 2: calculate evaluation for each neuron
        for (int i = 0; i < topology[2]; i++)
        {
            float summation = 0;

            for (int j = 0; j < topology[1]; j++)
            {
                //             (Preivous layers' evals)         (get current layers incoming weights)
                summation += layers[1].neurons[j].Evaluation * layers[2].neurons[i].incomingWeights[j];
            }
            

            layers[2].neurons[i].Evaluation = Activation(summation) - .5f;
        }

        // OUTPUT layer: calculate evaluation for each neuron
        for (int i = 0; i < topology[3]; i++)
        {
            float summation = 0;

            for (int j = 0; j < topology[2]; j++)
            {
                //             (Preivous layers' evals)         (get current layers incoming weights)
                summation += layers[2].neurons[j].Evaluation * layers[3].neurons[i].incomingWeights[j];
            }
            //print("Sum :" + Activation(summation));

            layers[3].neurons[i].Evaluation = Activation(summation) - .5f;
        }

        
        return new Vector2(layers[3].neurons[0].Evaluation, layers[3].neurons[1].Evaluation);
    }

    // Loops through entire network to set IncomingWeights
    public void SetRandomWeights()
    {
        // Netowrk 5-3-2
        // Loop through layers 3-2
        for (int k = 1; k < topology.Length; k++)
        {

            // Loop through neurons
            for (int i = 0; i < topology[k]; i++)
            {
                // init incoming weights
                for (int j = 0; j < topology[k-1]; j++)
                {
                    layers[k].neurons[i].incomingWeights.Add(Random.Range(-1f,1f));
                }
            }

        }
    }

    public void initializeLayers()
    {
        layers = new List<Layer>();

        // Creates 4 layers in the neural network
        for (int i = 0; i < topology.Length; i++)
        {
            layers.Add(new Layer());
        }

        // Fill each layer with 5-4-3-2 neuron topology

        for (int i = 0; i < topology.Length; i++)
        {
            for (int j = 0; j < topology[i]; j++)
            {

                layers[i].neurons.Add(new Neuron());
            }
        }

        //for each layer except the input layer
        for (int i = 1; i < 4; i++)
        {
            for (int j = 0; j < layers[i].neurons.Count; j++)
            {
                layers[i].neurons[j].incomingWeights = new List<float>();
            }
        }
    }

    // Activation function using Sigmoid curve formula
    public float Activation(float sum)
    {
        // Sigmoid exuation
        float ans = 1 / (1 + Mathf.Exp(-sum));

        return ans;
    }
}

public class Layer
{
    public float debug = 1;
    public List<Neuron> neurons;

    public Layer()
    {
        neurons = new List<Neuron>();
    }
}

public class Neuron
{
    public float Evaluation = 0;
    public float bias = 0;

    // These are the weight going to the neuron
    public List<float> incomingWeights;

    public Neuron()
    {
        incomingWeights = new List<float>();

    }
}