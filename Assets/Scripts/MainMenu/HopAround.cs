using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopAround : MonoBehaviour
{
    [SerializeField] GameObject startLimit;
    [SerializeField] GameObject endLimit;
    [SerializeField] Animatable animator;
    Vector3 moveRange;
    static List<HopAround> bouncers = new();
    private void Awake()
    {
        bouncers.Clear();
    }
    private void Start()
    {
        moveRange = endLimit.transform.position - startLimit.transform.position;
        bouncers.Add(this);
        transform.position = GetBounceDestination();
        StartCoroutine(DoSomething());
    }

    readonly float travelTime = .8f;
    readonly float maxTimeBeforeAction = 6f;
    readonly int decisionRatio = 6;
    IEnumerator DoSomething()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, maxTimeBeforeAction));
            int chooseAction = Random.Range(0, decisionRatio);
            if(chooseAction == 0) animator.Animate(AnimType.CAST);
            else if(chooseAction == 1) animator.Animate(AnimType.CHEER);
            else if (chooseAction >= 2)
            {
                Vector3 destination = GetBounceDestination();
                transform.LookAt(destination);
                animator.Animate(AnimType.JUMP);
                yield return new WaitForSeconds(.2f);
                yield return StartCoroutine(gameObject.LerpTo(destination, travelTime));
            }
        }
    }

    readonly float exclusionRange = .8f;
    Vector3 GetBounceDestination()
    {
        float destinationX = Random.Range(0, moveRange.x);
        float destinationY = Random.Range(0, moveRange.z);
        Vector3 destination = new(destinationX, 0, destinationY);
        destination += startLimit.transform.position;
        bool conflict = false;
        foreach(var bouncer in bouncers)
        {
            if (bouncer == this) continue;
            if (Vector3.Magnitude(bouncer.transform.position - destination) < exclusionRange) conflict = true;
        }
        if (conflict) destination = GetBounceDestination();
        return destination;
    }
}
