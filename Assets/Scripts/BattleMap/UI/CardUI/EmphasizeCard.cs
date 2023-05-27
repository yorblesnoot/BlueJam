using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmphasizeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public Vector3 originalPosition;

    [HideInInspector]
    public Vector3 originalScale;

    public Canvas mainCanvas;


    bool emphasis = false;

    [HideInInspector]
    public bool readyEmphasis = false;
    public float scaleFactor;
    public float positionFactor;

    EmphasizeCard()
    {
        scaleFactor = 1.5f;
        positionFactor = 1.3f;
    }
    
    public void OnPointerEnter(PointerEventData data)
    {
        if(emphasis == false && readyEmphasis == true)
        {
            originalPosition = transform.localPosition;
            originalScale = transform.localScale;
            emphasis = true;

            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(scale.x*scaleFactor,scale.y*scaleFactor,scale.z*scaleFactor);

            Vector3 position = transform.localPosition;
            RectTransform cardRect = gameObject.GetComponent<RectTransform>();
            positionFactor = cardRect.rect.height / positionFactor;
            transform.localPosition = new Vector3(position.x,position.y+positionFactor,position.z);
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (emphasis == true)
        {
            transform.localPosition = originalPosition;
            transform.localScale = originalScale;
            emphasis = false;
        }
    }

    private void OnDisable()
    {
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
        emphasis = false;
    }
}
