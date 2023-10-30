using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateFeedback : MonoBehaviour
{
    int hitLayer;
    int baseLayer;
    GameObject model;
    Vector3 baseScale;

    [SerializeField] GameObject floatNumber;
    public static ObjectPool numberPool;
    private void Awake()
    {
        if (numberPool == null) numberPool = new(floatNumber);
        string modelName = gameObject.name.Replace("NPC(Clone)", "");
        model = transform.Find(modelName).gameObject;
        baseScale = model.transform.localScale;
        hitLayer = LayerMask.NameToLayer("HitFlash");
        baseLayer = LayerMask.NameToLayer("Default");
    }

    public IEnumerator DamageFlash()
    {
        SetChildrenLayer(model, hitLayer);
        model.transform.localScale = baseScale * damagedScale;
        yield return new WaitForSeconds(flashDuration);
        SetChildrenLayer(model, baseLayer);
        model.transform.localScale = baseScale;
    }

    void SetChildrenLayer(GameObject target, int layer)
    {
        target.layer = layer;
        if (target.transform.childCount == 0) return;
        foreach (Transform child in target.transform)
        {
            SetChildrenLayer(child.gameObject, layer);
        }
    }

    static readonly float flashDuration = .2f;
    static readonly float damagedScale = 1.1f;
    static readonly float displacementFactor = .7f;
    void PopupFloatingNumber(int number, Color color, int displacement)
    {
        Vector3 popPosition = transform.position;
        popPosition.x += displacementFactor * (displacement);
        TMP_Text floatText = numberPool.InstantiateFromPool(popPosition, Quaternion.identity).GetComponentInChildren<TMP_Text>();
        floatText.text = number.ToString();
        floatText.color = color;
    }

    class Popup
    {
        public int number;
        public Color color;
    }

    Queue<Popup> popupQueue;

    public void QueuePopup(int number, Color color)
    {
        if (popupQueue == null || popupQueue.Count == 0)
        {
            popupQueue = new();
            StartCoroutine(SequencePopups());
        }
        popupQueue.Enqueue(new Popup {  number = number, color = color });
    }

    static float popDelay = .2f;
    private IEnumerator SequencePopups()
    {
        yield return null;
        while(popupQueue.Count > 0)
        {
            yield return new WaitForSeconds(popDelay);
            Popup newpop = popupQueue.Dequeue();
            PopupFloatingNumber(newpop.number, newpop.color, popupQueue.Count);
        }
    }
}
