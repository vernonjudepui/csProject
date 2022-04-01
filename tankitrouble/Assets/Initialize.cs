using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Initialize : MonoBehaviour
{
  public GameObject p1Fab;
  public GameObject p2Fab;
  public UnityEngine.UI.Text t1;
  public UnityEngine.UI.Text t2;
  // Start is called before the first frame update
  void Start()
  {
    GlobalVariables.p1Score = 0;
    GlobalVariables.p2Score = 0;
    // t1.text = GlobalVariables.p1Score + "";
    // t2.text = GlobalVariables.p2Score + "";
    GameObject p1 = Instantiate(p1Fab, transform.parent.position + new Vector3(-7, -4.5f, -1),
                                                transform.rotation);
    GameObject p2 = Instantiate(p2Fab, transform.parent.position + new Vector3(7, 4.5f, -1),
                                                transform.rotation);
    p1.transform.parent = transform;
    p2.transform.parent = transform;

  }

  // Update is called once per frame
  void Update()
  {

  }
  void LateUpdate()
  {
    t1.text = GlobalVariables.p1Score + "";
    t2.text = GlobalVariables.p2Score + "";
  }
}
