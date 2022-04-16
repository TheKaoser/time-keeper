using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed=0.1f;
 
    void Update ()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        transform.rotation =  Quaternion.Slerp(transform.rotation, target.rotation, rotationSpeed *  Time.deltaTime);
    }
}
