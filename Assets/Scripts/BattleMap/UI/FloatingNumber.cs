using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingNumber : MonoBehaviour
{
    [SerializeField] float startLift;
    private void Awake()
    {
        var randomParams = .5f;
        Vector3 newPosition = transform.position;
        newPosition.y += startLift;
        newPosition.x += UnityEngine.Random.Range(-randomParams, randomParams);
        newPosition.z += UnityEngine.Random.Range(-randomParams, randomParams);
        transform.position = newPosition;
        transform.LookAt(Camera.main.transform);
        StartCoroutine(AnimateFloatingNumber());
    }

    [SerializeField] float lifespan;
    private IEnumerator AnimateFloatingNumber()
    {
        yield return new WaitForSeconds(lifespan);
        gameObject.SetActive(false);
    }
}
