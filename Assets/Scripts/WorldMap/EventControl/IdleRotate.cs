using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRotate : MonoBehaviour
{
    [SerializeField] Vector3 rotationAxis;
    [SerializeField] float speed;
    void Update()
    {
        transform.Rotate(rotationAxis * speed *Time.deltaTime);
    }
}
