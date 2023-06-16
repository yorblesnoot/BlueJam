using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{
    Vector3 lockPointModifier = new Vector3 (7.92f, -6.47f, 7.92f);

    public void CameraLockOn()
    {
        GameObject lockPoint = GameObject.FindGameObjectWithTag("CameraLockPoint");
        if (lockPoint == null) Debug.Log("No camera lock point found. Try adding it to the Battle Map.");
        transform.position = lockPoint.transform.position-lockPointModifier;
    }
}
