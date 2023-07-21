using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachCamera : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(30, 45, 0);
    }
}