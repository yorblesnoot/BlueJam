using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnIndicator : MonoBehaviour
{
    [SerializeField] GameObject turnIndicator;
    [SerializeField] Animator animator;
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public IEnumerator ShowTurnExclamation()
    {
        if (TurnManager.turnTakers.Count == 0) yield break;
        turnIndicator.SetActive(true);
        animator.Play("ExclaWiggle");
        yield return new WaitForSeconds(.5f);
        turnIndicator.SetActive(false);

    }
}
