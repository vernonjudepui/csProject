using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float move;
    public float rotation;
    public float rotateSpeed = 200f;
    public float posx1 = -7f;
    public float posy1= -4.5f;

    public Rigidbody2D rb;
    void Start() {
        transform.position = new Vector3(posx1, posy1,-1);

    }
    // Update is called once per frame
    void Update()
    {
   
        //input   
        move = Input.GetAxis("Vertical");
        rotation = Input.GetAxis("Horizontal");
    }
    void FixedUpdate() {
        //movement
        transform.Translate(0f, move * moveSpeed * Time.fixedDeltaTime,0f);
        transform.Rotate(0f, 0f, rotation*-rotateSpeed * Time.fixedDeltaTime);

    }
    
}

