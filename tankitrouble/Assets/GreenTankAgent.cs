using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GreenTankAgent : Agent
{
  public GameObject enemyTank;
  public GameObject bulletPrefab;
  public Vector3 startPosition;
  public bool died;
  public bool killedEnemy;

  public override void OnEpisodeBegin()
  {
    Debug.Log("episode begin");
    died = false;
    killedEnemy = false;
    gameObject.transform.position = startPosition;
    gameObject.transform.rotation = Quaternion.Euler(Vector3.forward);
    gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
    // gameObject.GetComponent<Rigidbody2D>().
  }

  public override void CollectObservations(VectorSensor sensor)
  {
    // Target and Agent positions
    sensor.AddObservation(enemyTank.transform.localPosition);
    sensor.AddObservation(transform.rotation);
    sensor.AddObservation(transform.localPosition);

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
        // Debug.DrawRay(transform.position, direction.Rotate(angle) * distance, Color.red);
        distances[i] = distance;
      }
    }
    sensor.AddObservation(distances);
  }

  public override void OnActionReceived(ActionBuffers actionBuffers)
  {
    float moveSpeed = 5f;
    float rotateSpeed = 200f;
    // Actions, size = 2
    float move = actionBuffers.ContinuousActions[0];
    float rotation = actionBuffers.ContinuousActions[1];
    int fire = actionBuffers.DiscreteActions[0];

    Debug.Log(fire);

    transform.Translate(0f, move * moveSpeed * Time.fixedDeltaTime, 0f);
    transform.Rotate(0f, 0f, rotation * -rotateSpeed * Time.fixedDeltaTime);


    // fire
    if (fire == 1)
    {
      float launchSpeed = 1000f;
      GameObject ball = Instantiate(bulletPrefab, transform.position, transform.rotation);
      ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
    }

   float reward = 0f;
    // Rewards
    float distanceToTarget = Vector3.Distance(transform.localPosition, enemyTank.transform.localPosition);

    // Eliminated target
    //if (killedEnemy)
    //{
     // Debug.Log(gameObject.name + " eliminated target");
      //reward += 1000f;
 
      //SetReward(reward);
      //EndEpisode();
    //}

    // Got killed
    /*
    else if (died)
    {
      Debug.Log(gameObject.name + " died");
      
      SetReward(-100f+18f-distanceToTarget);
      EndEpisode();
    }
    */
    //else
    //{
      SetReward(18f - distanceToTarget);
   // }
  }

  public override void Heuristic(in ActionBuffers actionsOut)
  {
    var continuousActionsOut = actionsOut.ContinuousActions;
    continuousActionsOut[0] = Input.GetAxis("Vertical");
    continuousActionsOut[1] = Input.GetAxis("Horizontal");
    var discreteActionsOut = actionsOut.DiscreteActions;
    discreteActionsOut[0] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
  }
}