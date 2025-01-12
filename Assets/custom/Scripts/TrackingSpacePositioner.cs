using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSpacePositioner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0, -0.16f, 0.3f);
        this.transform.rotation = new Quaternion(0, 180, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
