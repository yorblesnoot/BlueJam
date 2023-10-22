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
    private void Awake()
    {
        string modelName = gameObject.name.Replace("NPC(Clone)", "");
        model = transform.Find(modelName).gameObject;
        baseScale = model.transform.localScale;
        hitLayer = LayerMask.NameToLayer("HitFlash");
        baseLayer = LayerMask.NameToLayer("Default");
    }

    public void DamagedState(int damage)
    {
        StartCoroutine(SingleFlash());
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
    IEnumerator SingleFlash()
    {
        SetChildrenLayer(model, hitLayer);
        model.transform.localScale = baseScale * damagedScale;
        yield return new WaitForSeconds(flashDuration);
        SetChildrenLayer(model, baseLayer);
        model.transform.localScale = baseScale;
    }

    public void PopupFloatingNumber(int number, Color color)
    {
        TMP_Text floatText = Instantiate(floatNumber, transform.localPosition, Quaternion.identity).GetComponentInChildren<TMP_Text>();
        floatText.text = number.ToString();
        floatText.color = color;
    }
}
