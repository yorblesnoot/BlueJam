using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachCamera : MonoBehaviour
{
    readonly int projectionDistance = 5;
    void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        transform.rotation = Quaternion.Euler(30,45,0);
        Vector3 direction = (Camera.main.transform.position - transform.position).normalized * projectionDistance;
        transform.position = transform.position + direction;
    }
}