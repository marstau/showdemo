using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject role = (GameObject)Resources.Load("Model/model1001001/model1001001");
        GameObject roleNode1 = GameObject.Find("Terrain/RoleNode1");
        role = Instantiate(role);
        role.transform.parent = roleNode1.transform;
        role.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
