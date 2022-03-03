using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProj : MonoBehaviour
{
    public GameObject projectile;
    public float launchSpeed = 1000f;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject ball = Instantiate(projectile, transform.position,
                                                    transform.rotation);
     
            Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), this.transform.parent.gameObject.transform.parent.gameObject.GetComponent<Collider2D>());
            ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
        }


    }
}