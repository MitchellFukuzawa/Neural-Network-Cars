using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cars : MonoBehaviour
{
    private bool initialized = false;
    private Transform car;

    private NeuralNetwork net;
    private Rigidbody rBody;
    private Material[] mats;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
        mats = new Material[transform.childCount];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    private void FixedUpdate()
    {
        if (initialized == true)
        {
            rBody.AddForce(new Vector3(10f, 0f, 0f));
        }
    }

    public void Init(NeuralNetwork net)
    {
        //this.car = hex;
        this.net = net;
        initialized = true;
    }
}
