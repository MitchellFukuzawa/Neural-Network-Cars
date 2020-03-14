using System.Collections;
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


    private const float Acceleration = 80f;
    private const float TurnRate = 100;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
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
}
