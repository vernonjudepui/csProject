using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootProjP2 : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameObject ball = Instantiate(projectile, transform.position,
                                                    transform.rotation);
            bulletBehaviour thing = ball.GetComponent<bulletBehaviour>();

            ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
            thing.FiredBy = "P2";
        }


    }
}
