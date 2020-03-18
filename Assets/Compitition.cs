using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Suppost to allow player to compete with trained ai
public class Compitition : MonoBehaviour
{
    public NeuralNetwork AI_Car;
    public GameObject player;

    private Checkpoints[] checkpoints;
    private float TrackLength;



    public float[,] weights = new float[6, 38];


    // Start is called before the first frame update
    void Start()
    {
        checkpoints = GetComponentsInChildren<Checkpoints>();

        GameObject inst = AI_Car.gameObject;

        AI_Car.initializeLayers();

        inst.transform.position = checkpoints[0].transform.position;
        player.transform.position = checkpoints[0].transform.position;

        int[] mathArray = AI_Car.topology;
        int numberOfAddedWeights = 0;

        // for all layers but the input layer
        // and setting the car weights
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


        //  load in to the car weights from text file
        SetWeights();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeights()
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
    }
}
