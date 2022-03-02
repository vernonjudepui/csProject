using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2movement: MonoBehaviour
{
    public float moveSpeed = 5f;
    public float move;
    public float rotation;
    public float rotateSpeed = 200f;
    public Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        move = 0;
        rotation = 0;
        if (Input.GetKey(KeyCode.W))
        {
            move += 1;
        
        }
         if (Input.GetKey(KeyCode.A))
        {

            rotation -= 1;

        }
         if (Input.GetKey(KeyCode.S))
        {

            move -= 1;

        }
         if (Input.GetKey(KeyCode.D))
        {

            rotation += 1;

        }
        //input   
        //move = Input.GetAxis("Vertical");
        //rotation = Input.GetAxis("Horizontal");
    }
    void FixedUpdate()
    {
        //movement
        transform.Translate(0f, move * moveSpeed * Time.fixedDeltaTime, 0f);
        transform.Rotate(0f, 0f, rotation * -rotateSpeed * Time.fixedDeltaTime);

    }

}

