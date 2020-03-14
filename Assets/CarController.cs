using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody carRigidbody;

    private const float Acceleration = 80f;
    private const float TurnRate = 100;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
