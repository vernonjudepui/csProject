using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bulletBehaviour : MonoBehaviour
{
    public GameObject p1Fab;
    public GameObject p2Fab;
    public UnityEngine.UI.Text t1;
    public UnityEngine.UI.Text t2;
    public string FiredBy = "";
    private float initTime;
    // Start is called before the first frame update
    void Start()
    {
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public float lifetime = 4.0f;

    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.name == "Bullet(Clone)")
        {
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
        if (Time.time - initTime > 0.015)
        {
            Debug.Log(Time.time - initTime + "asdasdasdadadadad");
            if (col.gameObject.name == "tank(Clone)")
            {
                // Destroy(col.gameObject);
                col.gameObject.GetComponent<TankAgent>().died = true;
                Debug.Log(FiredBy);
                if (FiredBy == "p2(Clone)")

                    GameObject.Find("p2(Clone)").GetComponent<TankAgent>().killedEnemy = true;
                Destroy(this.gameObject);
                col.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                col.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
                GlobalVariables.p2Score += 1;
                Debug.Log(GlobalVariables.p2Score);
                UpdateScore();

                // GameObject hand = GameObject.Find("p2(Clone)");
                // GameObject p1 = Instantiate(p1Fab, new Vector3(-7f, -4.5f, -1),
                //                                     Quaternion.Euler(Vector3.forward));
                // hand.transform.rotation = Quaternion.Euler(Vector3.forward);
                // hand.transform.localPosition = new Vector3(7f, 4.5f, -1);
            }


            if (col.gameObject.name == "p2(Clone)")
            {
                // Destroy(col.gameObject);

                col.gameObject.GetComponent<TankAgent>().died = true;
                if (FiredBy == "tank(Clone)")
                    GameObject.Find("tank(Clone)").GetComponent<TankAgent>().killedEnemy = true;
                Destroy(this.gameObject);
                col.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                col.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
                GlobalVariables.p1Score += 1;
                UpdateScore();
                // GameObject tank = GameObject.Find("tank(Clone)");
                // GameObject p2 = Instantiate(p2Fab, new Vector3(7f, 4.5f, -1),
                //                                             Quaternion.Euler(Vector3.forward));
                // tank.transform.rotation = Quaternion.Euler(Vector3.forward);
                // tank.transform.localPosition = new Vector3(-7f, -4.5f, -1);
            }

        }
    }
    void UpdateScore()
    {
        t1.text = GlobalVariables.p1Score + "";
        t2.text = GlobalVariables.p2Score + "";

    }
    void Awake()
    {
        Destroy(this.gameObject, lifetime);
    }
}
