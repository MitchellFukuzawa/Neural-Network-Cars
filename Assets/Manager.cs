using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject carPrefab;

    private List<NeuralNetwork> nets;
    private List<Cars> carList = null;
    private bool isTraining = false;
    private int population = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; // 1 input and 1 output

    private void Update()
    {
        if (isTraining == false)
        {
            if (generationNumber == 0)
            {
                InitNeuralNetworks();
            }
            else
            {
                nets.Sort();
                for (int i = 0; i < population / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (population / 2)]);
                    nets[i].Mutate();

                    nets[i + (population / 2)] = new NeuralNetwork(nets[i + (population / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < population; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;

            isTraining = true;
            Invoke("Timer", 15f);
            CreateCarBodies();
        }
    }

    private void CreateCarBodies()
    {
        if (carList != null)
        {
            for (int i = 0; i < carList.Count; i++)
            {
                GameObject.Destroy(carList[i].gameObject);
            }
        }

        carList = new List<Cars>();

        for (int i = 0; i < population; i++)
        {
            // Instantiate cars
            Cars car = ((GameObject)Instantiate(carPrefab, new Vector3(0f, 0f, 0f), carPrefab.transform.rotation)).GetComponent<Cars>();
            car.Init(nets[i]);
            carList.Add(car);
        }
    }

    void InitNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (population % 2 != 0)
        {
            population = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < population; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
