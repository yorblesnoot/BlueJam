using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloatingNumber : MonoBehaviour
{
    [SerializeField] float startLift;
    [SerializeField] float rotationRange;

    private void OnEnable()
    {
        Vector3 newPosition = transform.position;
        newPosition.y += startLift;
        transform.position = newPosition;
        transform.LookAt(Camera.main.transform);
        transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, 1f);

        Quaternion rotationMod = Quaternion.identity; //Quaternion.Euler(0, 0, Random.Range(-rotationRange * 2, rotationRange));
        transform.rotation = transform.rotation * rotationMod;
        StartCoroutine(AnimateFloatingNumber());
    }

    [SerializeField] float lifespan;
    private IEnumerator AnimateFloatingNumber()
    {
        yield return new WaitForSeconds(lifespan);
        StateFeedback.numberPool.ReturnToPool(gameObject);
    }
}
