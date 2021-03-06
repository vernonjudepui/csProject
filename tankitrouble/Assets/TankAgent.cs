using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TankAgent : Agent
{
    public GameObject enemyTank;
    public GameObject bulletPrefab;
    public Vector3 startPosition;
    public bool died;
    public bool killedEnemy;

    private float timeTaken = 0f;
    private float timeTillNextFire = 0f;

    public override void OnEpisodeBegin()
    {
        timeTaken = 0f;
        timeTillNextFire = 0f;
        Debug.Log("episode begin");
        died = false;
        killedEnemy = false;
        gameObject.transform.localPosition = startPosition;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.forward + new Vector3(0f, 0f, 20f));
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        // temporary
        //enemyTank.transform.localPosition = new Vector3(Random.Range(-7, 7), Random.Range(-7, 7), -1);
        // enemyTank.transform.rotation = Quaternion.Euler(Vector3.forward);
        // enemyTank.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //enemyTank.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        // clear all bullets
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("bullet"))
        {
            Destroy(bullet);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(enemyTank.transform.position);
        sensor.AddObservation(transform.rotation.eulerAngles[2]);
        sensor.AddObservation(transform.rotation * Vector2.up);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(IsTargetInLineOfSight());
        if (timeTaken > timeTillNextFire)
        {
            sensor.AddObservation(true);
        }
        else
        {
            sensor.AddObservation(false);
        }

        float distanceToTarget = Vector3.Distance(transform.localPosition, enemyTank.transform.localPosition);
        sensor.AddObservation(distanceToTarget);
        // Distance to walls in 8 directions
        float[] distances = new float[8];
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45;
            Vector2 direction = (transform.rotation * Vector2.up);
            RaycastHit2D[] results = new RaycastHit2D[16];
            if (Physics2D.Raycast(transform.localPosition, direction.Rotate(angle), new ContactFilter2D().NoFilter(), results, 100f) > 0)
            {
                float distance = Vector2.Distance(transform.localPosition, results[1].point);
                // Debug.DrawRay(transform.localPosition, direction.Rotate(angle) * distance, Color.red);
                distances[i] = distance;
            }
        }
        sensor.AddObservation(distances);

        GameObject[] allBullets = GameObject.FindGameObjectsWithTag("bullet");
        int numClosest = 4; // gives position of 4 nearest bullets

        SortedList<float, GameObject> bulletDistances = new SortedList<float, GameObject>(numClosest);
        foreach (GameObject bullet in allBullets)
        {
            bulletDistances.Add(Vector2.Distance(bullet.transform.position, transform.position), bullet);
        }
        foreach (GameObject bullet in bulletDistances.Values)
        {
            sensor.AddObservation(bullet.transform.localPosition);
            sensor.AddObservation(bullet.GetComponent<Rigidbody2D>().velocity);
        }


        AddReward(-0.004f);

    }

    bool IsTargetInLineOfSight()
    {
        Vector2 direction = (transform.rotation * Vector2.up);
        //Debug.Log("DIRECTRION"+direction);

        RaycastHit2D[] results = new RaycastHit2D[4];
        if (Physics2D.Raycast(transform.position, direction, new ContactFilter2D().NoFilter(), results, 20f) > 0)
        {
            if (results[1].collider == null)
                return false;
            if (Equals(results[1].collider.gameObject.transform.name, enemyTank.transform.name + "(Clone)"))
            {
                Debug.Log("I SEE YOU");
                return true;
            }
            // Debug.Log("WHISITS"+results[1].collider.gameObject.transform);
            //Debug.Log("WW"+enemyTank.transform.name);
        }

        Debug.Log("I DONT SEE YOU");
        return false;
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        float moveSpeed = 1f; //5f;
        float rotateSpeed = 40f; // 200f;

        // Actions, size = 2
        int fire = actionBuffers.DiscreteActions[0];
        float move = actionBuffers.DiscreteActions[1] - 1;
        float rotation = actionBuffers.DiscreteActions[2] - 1;

        // Debug.Log(fire);

        transform.Translate(0, move * moveSpeed * Time.fixedDeltaTime, 0f);
        transform.Rotate(0f, 0f, rotation * rotateSpeed * Time.fixedDeltaTime);

        Vector2 direction = (transform.rotation * Vector2.up);
        // fire
        if (IsTargetInLineOfSight())
            AddReward(0.01f);
        if (fire == 1 && timeTaken > timeTillNextFire)
        {
            float launchSpeed = 300f;
            Debug.Log(transform.position);

            GameObject ball = Instantiate(bulletPrefab, transform.position + new Vector3(direction[0] * 0.63f, direction[1] * 0.63f, -1f), transform.rotation);
            ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
            // ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, launchSpeed);
            bulletBehaviour thing = ball.GetComponent<bulletBehaviour>();
            thing.FiredBy = gameObject.name;
            AddReward(0.2f);
            Debug.Log("Fired by" + gameObject.name);
            timeTillNextFire = timeTaken + 200f;
        }
        else if (fire == 1)
        {
            AddReward(-0.005f);
            Debug.Log("False Fired by" + gameObject.name + fire);
        }
        else if (fire == 0)
        {
            Debug.Log("not False Fired by" + gameObject.name + fire);

        }
        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, enemyTank.transform.localPosition);
        //Debug.Log(distanceToTarget+"asadasd");
        //Eliminated target
        if (killedEnemy)
        {

            AddReward(4f + 2 - timeTaken / 2000);
            EndEpisode();
        }
        timeTaken += 1;
        if (timeTaken > 2000)
        {
            AddReward(-distanceToTarget / 10f);
            EndEpisode();
        }
        // // Got killed
        else if (died)
        {
            Debug.Log(gameObject.name + " died");

            AddReward(-4f - distanceToTarget / 10f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // var continuousActionsOut = actionsOut.ContinuousActions;
        // continuousActionsOut[0] = Input.GetAxis("Vertical");
        // continuousActionsOut[1] = Input.GetAxis("Horizontal");
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
        discreteActionsOut[1] = Input.GetKeyDown(KeyCode.UpArrow) ? 2 : (Input.GetKeyDown(KeyCode.DownArrow) ? 0 : 1);
        discreteActionsOut[2] = Input.GetKeyDown(KeyCode.LeftArrow) ? 0 : (Input.GetKeyDown(KeyCode.RightArrow) ? 2 : 1);
    }
}
// }using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Sensors;
// using Unity.MLAgents.Actuators;

// public class TankAgent : Agent
// {
//   public GameObject enemyTank;
//   public GameObject bulletPrefab;
//   public Vector3 startPosition;
//   public bool died;
//   public bool killedEnemy;

//   private float timeTaken = 0f;


//   public override void OnEpisodeBegin()
//   {
//     timeTaken = 0f;
//     Debug.Log("episode begin");
//     died = false;
//     killedEnemy = false;
//     gameObject.transform.localPosition = new Vector3(-7, -7, -1);
//     gameObject.transform.rotation = Quaternion.Euler(Vector3.forward);
//     gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
//     gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
//     // temporary
//     //enemyTank.transform.localPosition = new Vector3(Random.Range(-7, 7), Random.Range(-7, 7), -1);
//    // enemyTank.transform.rotation = Quaternion.Euler(Vector3.forward);
//    // enemyTank.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
//    //enemyTank.GetComponent<Rigidbody2D>().angularVelocity = 0f;
//   }

//   public override void CollectObservations(VectorSensor sensor)
//   {
//     // Target and Agent positions
//     sensor.AddObservation(enemyTank.transform.localPosition);
//     sensor.AddObservation(transform.rotation.eulerAngles[2]);
//     sensor.AddObservation(transform.rotation * Vector2.up);
//     sensor.AddObservation(transform.localPosition);

//     // Distance to walls in 8 directions
//     float[] distances = new float[8];
//     for (int i = 0; i < 8; i++)
//     {
//       float angle = i * 45;
//       Vector2 direction = (transform.rotation * Vector2.up);
//       RaycastHit2D[] results = new RaycastHit2D[16];
//       if (Physics2D.Raycast(transform.localPosition, direction.Rotate(angle), new ContactFilter2D().NoFilter(), results, 100f) > 0)
//       {
//         float distance = Vector2.Distance(transform.localPosition, results[1].point);
//         // Debug.DrawRay(transform.localPosition, direction.Rotate(angle) * distance, Color.red);
//         distances[i] = distance;
//       }
//     }
//     sensor.AddObservation(distances);

//     AddReward(-0.01f);
//     timeTaken += 1;
//     if (timeTaken > 2000)
//     {
//       float distanceToTarget = Vector3.Distance(transform.localPosition, enemyTank.transform.localPosition);
//       AddReward(-distanceToTarget / 20f);
//       EndEpisode();
//     }
//   }

//   public override void OnActionReceived(ActionBuffers actionBuffers)
//   {
//     float moveSpeed = 5f; //5f;
//     float rotateSpeed = 200f; // 200f;

//     // Actions, size = 2
//     float move = actionBuffers.ContinuousActions[0];
//     float rotation = actionBuffers.ContinuousActions[1];
//     // int fire = actionBuffers.DiscreteActions[0];

//     // Debug.Log(fire);

//     transform.Translate(0, move * moveSpeed * Time.fixedDeltaTime, 0f);
//     transform.Rotate(0f, 0f, rotation * rotateSpeed * Time.fixedDeltaTime);


//     // fire
//     if (fire == 1)
//     {
//       float launchSpeed = 1000f;
//        GameObject ball = Instantiate(bulletPrefab, transform.localPosition, transform.rotation);
//        ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
//      }

//     // Rewards
//     float distanceToTarget = Vector3.Distance(transform.localPosition, enemyTank.transform.localPosition);
//     //Eliminated target
//     if (killedEnemy)
//     {
//      Debug.Log(gameObject.name + " eliminated target");


//       AddReward(10f);
//       EndEpisode();
//     }

//     // // Got killed
//      else if (died)
//     {
//        Debug.Log(gameObject.name + " died");

//       AddReward(-10f-distanceToTarget/20);
//        EndEpisode();
//      }


//   }

//   public override void Heuristic(in ActionBuffers actionsOut)
//   {
//     var continuousActionsOut = actionsOut.ContinuousActions;
//     continuousActionsOut[0] = Input.GetAxis("Vertical");
//     continuousActionsOut[1] = Input.GetAxis("Horizontal");
//     var discreteActionsOut = actionsOut.DiscreteActions;
//     discreteActionsOut[0] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
//   }


//   void OnCollisionEnter2D(Collision2D col)
//   {
//     if (col.gameObject.name == "p2(Clone)")
//     {
//       AddReward(3f);
//       EndEpisode();
//     }
//     else if (col.gameObject.tag == "wall")
//     {
//       float distanceToTarget = Vector3.Distance(transform.localPosition, enemyTank.transform.localPosition);
//       AddReward(-2f - distanceToTarget / 20f);
//       EndEpisode();
//     }
//   }
// }