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
    public Manager manager;

    public float vertMove;
    public float horizontalMove;

    public NeuralNetwork NN;

    private const float Acceleration = 80f;
    private const float TurnRate = 100;
    private int currentCheckpoint = 0;
    private Checkpoints[] checkpoints;
    private float TrackLength;
    private float carSpeed;
    public bool isCarDead = false;
    public bool UseUserInput = false;

    // Add the cars speed into this list to then be averaged when it dies
    public List<float> SpeedDuringSimulation;

    // Parts of the fitness evaluation
    private float Percentage = 0;
    private float AverageSpeed = 0;
    private float t_TimeAlive = 0;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        manager = Manager._instance;
        checkpoints = manager.GetComponentsInChildren<Checkpoints>();
        CalculateCheckpointPercentages();
        NN = GetComponent<NeuralNetwork>();
        SpeedDuringSimulation = new List<float>();

        // Speed up simulation
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        float WeightVerticle = 0;
        float WeightHorizontal = 0;
        

        float[] raycastDistances = new float[5];

        raycastDistances[0] = CreateRayCast(rayLeft, 100, -transform.right);
        raycastDistances[1] = CreateRayCast(rayLeft, 100, -transform.right + transform.forward);
        raycastDistances[2] = CreateRayCast(rayLeft, 100, transform.right + transform.forward);
        raycastDistances[3] = CreateRayCast(rayLeft, 100, transform.right);
        raycastDistances[4] = CreateRayCast(rayLeft, 100, transform.forward);

        raycastDistances[0] *= -1;
        raycastDistances[1] *= -1;


        // Get the AI values
        Vector2 AI_Movement = NN.ProccessingEvaluations(raycastDistances);
        WeightVerticle = AI_Movement.x + .2f;
        WeightHorizontal = (AI_Movement.y);
        #region Input
        // USER input
        //WeightVerticle = Input.GetAxis("Vertical");
        //WeightHorizontal = Input.GetAxis("Horizontal");

        // Clamp User Input weights
        WeightVerticle = Mathf.Clamp(WeightVerticle, -1, 1);
        WeightHorizontal = Mathf.Clamp(WeightHorizontal, -1, 1);

        //Document Speed for avg speed calc
        carSpeed = WeightVerticle;

        // Move forward
        if (!isCarDead)
        {
            carRigidbody.velocity = WeightVerticle * transform.forward * Acceleration;
            carRigidbody.transform.Rotate(Vector3.up * WeightHorizontal * TurnRate * Time.deltaTime);

            // Average speed list
            CollectSpeedData(0.25f, SpeedDuringSimulation);

            t_TimeAlive += Time.deltaTime;


            // Do some checks to see if the car is sitting in place
            if (Mathf.Abs(WeightVerticle) < .1f && t_TimeAlive > 7f && SpeedDuringSimulation.Count >= 2)
                isCarDead = true;

            // check 
            if (t_TimeAlive > 7f && Percentage < .1f && SpeedDuringSimulation.Count >= 2)
                isCarDead = true;

            // Edge case to kill everything after 30 seconds
            if (t_TimeAlive > 60f && SpeedDuringSimulation.Count >= 2)
                isCarDead = true;

        }
        else
        {
            carRigidbody.velocity = Vector3.zero;
        }
        #endregion


        // Percentage Calculation
        int temp = currentCheckpoint - 1;

        Percentage = Vector2.Distance(checkpoints[currentCheckpoint - 1].transform.position, transform.position);
        //Debug.Log("Current chekpoint to current position : " + Percentage);
        if (currentCheckpoint != 1)
            Percentage += checkpoints[currentCheckpoint - 1].AccumulatedDistance;
        Percentage /= TrackLength;

        // Clamp percentage to 100 if last checkpoint is reached
        if (currentCheckpoint == checkpoints.Length || Percentage > 1f)
            Percentage = 1f;

    }

    // Timer used with Colltion of speed data
    float timeSinceLastSpeedCapture = 0;
    // Function to record the car speed every capture rate and add into speeds
    private void CollectSpeedData(float captureRate ,List<float> speeds)
    {
        timeSinceLastSpeedCapture += Time.deltaTime;

        if(timeSinceLastSpeedCapture >= captureRate)
        {
            speeds.Add(carSpeed);
            timeSinceLastSpeedCapture = 0;
        }
    }

    // Call this when the car dies using the list of accumulated speeds
    public float CalculateAvgSpeed(List<float> speeds)
    {
        float sum = 0;
        float avg = 0;

        foreach (var speed in speeds)
        {
            sum += speed;
        }

        // Make sure we arent div by 0
        if (sum != 0)
            avg = sum / speeds.Count > 1 ? speeds.Count : .1f;
        return avg;
    }

    // Calc fitness and set to neural network
    private void SendFitness()
    {
        NN.fitness = CalculateAvgSpeed(SpeedDuringSimulation) * .75f + Percentage * 1f;
    }

    // Raycast for wall detection
    private float CreateRayCast(Transform position, float dist, Vector3 direction)
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 9;
        layerMask |= 1 << 8;


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
            return 100;
        }
    }

    // part of the fitness evaluation
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
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            //Stop the car

            isCarDead = true;

            SendFitness();
        }
    }
}
