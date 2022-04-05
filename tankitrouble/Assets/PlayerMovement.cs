using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension {

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;
    public float move;
    public float rotation;
    public float rotateSpeed = 200f;
    public float posx1 = -7f;
    public float posy1 = -4.5f;

    public Rigidbody2D rb;
    void Start() {
        // transform.localPosition = new Vector3(posx1, posy1, -1);

    }
    // Update is called once per frame
    void Update() {

        //input   
        move = Input.GetAxis("Vertical");
        rotation = Input.GetAxis("Horizontal");
        // for (int i = 0; i < 8; i++)
        // {
        //   float angle = i * 45;
        //   Vector2 direction = (transform.rotation * Vector2.up);
        //   RaycastHit2D[] results = new RaycastHit2D[16];
        //   if (Physics2D.Raycast(transform.localPosition, direction.Rotate(angle), new ContactFilter2D().NoFilter(), results, 100f) > 0)
        //   {
        //     // Vector2 point = results[1].collider.gameObject.name == "p2(Clone)" ? (Vector2)(results[1].transform.localPosition) : results[1].point;
        //     float distance = Vector2.Distance(transform.localPosition, results[1].point);
        //     Debug.DrawRay(transform.localPosition, direction.Rotate(angle) * distance, Color.red);
        //   }
        // }
    }
    void FixedUpdate() {
        //movement
        transform.Translate(0f, move * moveSpeed * Time.fixedDeltaTime, 0f);
        transform.Rotate(0f, 0f, rotation * -rotateSpeed * Time.fixedDeltaTime);

    }

}

