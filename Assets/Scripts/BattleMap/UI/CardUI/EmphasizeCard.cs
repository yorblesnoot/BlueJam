using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EmphasizeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public Vector3 originalPosition;

    [HideInInspector]
    public Vector3 originalScale;

    bool emphasis = false;

    public bool readyEmphasis = false;

    float scaleFactor = 1.5f;
    float screenHeightFactor = 1.35f;

    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            scaleFactor = 1.5f;
            screenHeightFactor = 1.4f;
            RectTransform cardRect = gameObject.GetComponent<RectTransform>();
            screenHeightFactor = cardRect.rect.height / screenHeightFactor;
        }
        else
        {
            scaleFactor = 1.05f;
            screenHeightFactor = 0f;
        }
    }
    public void OnPointerEnter(PointerEventData data)
    {
        if(emphasis == false && readyEmphasis == true)
        {
            //save original position and scale
            emphasis = true;

            //emphasize scale
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(scale.x*scaleFactor,scale.y*scaleFactor,scale.z*scaleFactor);

            //pop up
            if (screenHeightFactor > 0f)
            {
                Vector3 position = transform.localPosition;
                transform.localPosition = new Vector3(position.x, position.y + screenHeightFactor, position.z);
            }
            //StartCoroutine(BlockEmphasis());
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        Deemphasize();
    }

    private void OnDisable()
    {
        Deemphasize();
    }

    void Deemphasize()
    {
        if (emphasis == true)
        {
            if (screenHeightFactor > 0f) transform.localPosition = originalPosition;
            transform.localScale = originalScale;
            emphasis = false;
        }
    }
}
