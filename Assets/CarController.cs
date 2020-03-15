﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody carRigidbody;

    public Transform rayLeft;
    public Transform rayLeftFront;
    public Transform rayFront;
    public Transform rayRightFront;
    public Transform rayRight;
    public GameObject managerRef;


    private const float Acceleration = 80f;
    private const float TurnRate = 100;
    private int currentCheckpoint = 0;
    private Checkpoints[] checkpoints;
    private float TrackLength;
    private float Percentage = 0;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        checkpoints = managerRef.GetComponentsInChildren<Checkpoints>();
        CalculateCheckpointPercentages();
    }

    // Update is called once per frame
    void Update()
    {
        CreateRayCast(rayLeft, Mathf.Infinity, -transform.right);
        CreateRayCast(rayLeftFront, Mathf.Infinity, -transform.right + transform.forward);
        CreateRayCast(rayLeft, Mathf.Infinity, transform.forward);
        CreateRayCast(rayLeft, Mathf.Infinity, transform.right + transform.forward);
        CreateRayCast(rayLeft, Mathf.Infinity, transform.right);


        float WeightVerticle = 0;
        float WeightHorizontal = 0;

        // User input
        WeightVerticle = Input.GetAxis("Vertical");
        WeightHorizontal = Input.GetAxis("Horizontal");

        // Clamp User Input weights
        Mathf.Clamp(WeightVerticle, -1, 1);
        Mathf.Clamp(WeightHorizontal, -1, 1);

        // Move forward
        carRigidbody.velocity = WeightVerticle * transform.forward * Acceleration;
        carRigidbody.transform.Rotate(Vector3.up * WeightHorizontal * TurnRate * Time.deltaTime);

        // Percentage Calculation
        Percentage = Vector2.Distance(checkpoints[currentCheckpoint - 1].transform.position, transform.position);
        //Debug.Log("Current chekpoint to current position : " + Percentage);
        if (currentCheckpoint != 1)
            Percentage += checkpoints[currentCheckpoint - 1].AccumulatedDistance;
        Percentage /= TrackLength;

        // Clamp percentage to 100 if last checkpoint is reached
        if (currentCheckpoint == checkpoints.Length || Percentage > 1f)
            Percentage = 1f;
        Debug.Log(Percentage);
    }



    // Raycast for wall detection
    private float CreateRayCast(Transform position, float dist, Vector3 direction)
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        //Vector3.ClampMagnitude(direction, 1);

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(position.position, direction, out hit, dist, layerMask))
        {
            Debug.DrawRay(position.position, direction.normalized * hit.distance, Color.green);
            return hit.distance;
        }
        else
        {
            Debug.DrawRay(position.position, direction.normalized * dist, Color.red);
            return 1;
        }
    }

    private void CalculateCheckpointPercentages()
    {
        checkpoints[0].AccumulatedDistance = 0;
        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].DistanceToPrevious = Vector2.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
            checkpoints[i].AccumulatedDistance = checkpoints[i - 1].AccumulatedDistance + checkpoints[i].DistanceToPrevious;
        }
        TrackLength = checkpoints[checkpoints.Length - 1].AccumulatedDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            //Check if collision is the next checkpoint it must hit
            if (other.gameObject.GetComponent<Checkpoints>().CheckpointNumber == currentCheckpoint)
            {
                currentCheckpoint++;
                //Debug.Log("PLUS");
            }
            else if (other.gameObject.GetComponent<Checkpoints>().CheckpointNumber == currentCheckpoint - 1 && currentCheckpoint != 1)
            {
                currentCheckpoint--;
                //Debug.Log("MINUS");
            }
        }
    }
}
