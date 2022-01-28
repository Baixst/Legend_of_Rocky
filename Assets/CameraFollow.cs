using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject objectToFollow;
    public Rigidbody rb;

void Start(){
    transform.parent = null;
}

    void FixedUpdate()
    {
        rb.velocity = 10f * (objectToFollow.transform.position - transform.position) * Vector3.Distance(objectToFollow.transform.position, transform.position);
        transform.position = new Vector3(transform.position.x,transform.position.y, -1f);
    }
}
