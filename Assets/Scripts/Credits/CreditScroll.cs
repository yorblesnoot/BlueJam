using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditScroll : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GridLayoutGroup gridLayout;
    readonly float timePerRow = 2;
    Vector3 scrollTarget;
    private void Awake()
    {
        int childCount = transform.childCount - 2;
        float scrollDistance = childCount * gridLayout.cellSize.y;
        scrollTarget = new(transform.position.x, transform.position.y + scrollDistance, transform.position.z);
        StartCoroutine(gameObject.LerpTo(scrollTarget, timePerRow * childCount));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        transform.position = scrollTarget;
    }
}
