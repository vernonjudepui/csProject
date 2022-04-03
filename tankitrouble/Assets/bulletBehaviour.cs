using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bulletBehaviour : MonoBehaviour {
    public GameObject p1Fab;
    public GameObject p2Fab;
    public UnityEngine.UI.Text t1;
    public UnityEngine.UI.Text t2;
    public string FiredBy = "";

    void Start() {
    }

    void Update() {

    }
    public float lifetime = 4.0f;

    void OnCollisionEnter2D(Collision2D col) {

        if (col.gameObject.name == "Bullet(Clone)") {
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
        if (col.gameObject.name == "tank") {
            Destroy(this.gameObject);
            // col.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // col.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
            GlobalVariables.p2Score += 1;
            UpdateScore();
        }


        if (col.gameObject.name == "p2") {
            Destroy(this.gameObject);
            // col.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // col.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
            GlobalVariables.p1Score += 1;
            UpdateScore();
        }
    }

    void UpdateScore() {
        t1.text = GlobalVariables.p1Score + "";
        t2.text = GlobalVariables.p2Score + "";

    }
    void Awake() {
        Destroy(this.gameObject, lifetime);
    }
}
