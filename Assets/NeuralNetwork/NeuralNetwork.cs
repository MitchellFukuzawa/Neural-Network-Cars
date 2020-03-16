using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public List<Layer> layers;
    [SerializeField]
    private int[] topology = new int[] { 4, 3, 2 };

    public void Start()
    {
        initializeLayers();
        SetRandomWeights();
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
        //for (int i = 0; i < 4; i++) // For all neurons in layers[1] to calculate each neuron evaluation
        //{
        //    float summation = 0;

        //    for (int j = 0; j < 5; j++) // For all incoming inputEvaluation * the neurons incoming weights are
        //    {
        //        summation += layers[0].neurons[j].Evaluation * layers[1].neurons[i].incomingWeights[j]; 
        //    }

        //    // set the summation of our neuron to the final summation value
        //    layers[1].neurons[i].Evaluation = Activation(summation);
        //}

        // Hidden layer 2: calculate evaluation for each neuron
        for (int i = 0; i < topology[1]; i++)
        {
            float summation = 0;

            for (int j = 0; j < topology[0]; j++)
            {
                //             (Preivous layers' evals)         (get current layers incoming weights)
                summation += layers[0].neurons[j].Evaluation * layers[1].neurons[i].incomingWeights[j];
            }
            

            layers[1].neurons[i].Evaluation = Activation(summation);
        }

        // OUTPUT layer: calculate evaluation for each neuron
        for (int i = 0; i < topology[2]; i++)
        {
            float summation = 0;

            for (int j = 0; j < topology[1]; j++)
            {
                //print("SizeofLayer 2: " + layers[2].neurons.Count);
                //print("SizeofLayer 3: " + layers[3].neurons[i].incomingWeights.Count);
                
                //             (Preivous layers' evals)         (get current layers incoming weights)
                summation += layers[1].neurons[j].Evaluation * layers[2].neurons[i].incomingWeights[j];
            }
            //print("Sum :" + Activation(summation));

            layers[2].neurons[i].Evaluation = Activation(summation);
        }

        
        return new Vector2(layers[2].neurons[0].Evaluation, layers[2].neurons[1].Evaluation);
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
                    layers[k].neurons[i].incomingWeights.Add(.5f);
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

        // Fill each layer with 5-3-2 neuron topology

        for (int i = 0; i < topology.Length; i++)
        {
            for (int j = 0; j < topology[i]; j++)
            {

                layers[i].neurons.Add(new Neuron());
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

    // These are the weight going to the neuron
    public List<float> incomingWeights;

    public Neuron()
    {
        incomingWeights = new List<float>();
    }
}