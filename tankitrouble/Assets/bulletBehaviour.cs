using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float lifetime = 4.0f;
    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.name=="Bullet(Clone)"){
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
//        if(col.gameO)
    }
    void Awake()
    {
        Destroy(this.gameObject, lifetime);
    }
}
