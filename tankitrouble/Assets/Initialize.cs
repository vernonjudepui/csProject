using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    public GameObject p1Fab;
    public GameObject p2Fab;
    // Start is called before the first frame update
    void Start()
    {
       GameObject p1 = Instantiate(p1Fab, new Vector3(-7f,-4.5f,-1),
                                                    transform.rotation);
        GameObject p2 = Instantiate(p2Fab, new Vector3(7f, 4.5f,-1),
                                                    transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
