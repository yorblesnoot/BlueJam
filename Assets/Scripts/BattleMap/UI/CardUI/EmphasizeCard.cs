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
    float positionFactor = 1.3f;

    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            scaleFactor = 1.5f;
            positionFactor = 1.3f;
            RectTransform cardRect = gameObject.GetComponent<RectTransform>();
            positionFactor = cardRect.rect.height / positionFactor;
        }
        else
        {
            scaleFactor = 1.05f;
            positionFactor = 0f;
        }
    }

    public void PrepareForEmphasis()
    {
        readyEmphasis = true;
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
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
            if (positionFactor > 0f)
            {
                Vector3 position = transform.localPosition;
                transform.localPosition = new Vector3(position.x, position.y + positionFactor, position.z);
            }
            //StartCoroutine(BlockEmphasis());
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (emphasis == true)
        {
            if(positionFactor > 0f) transform.localPosition = originalPosition;
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
