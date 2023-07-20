using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialBubble : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] List<GameObject> components;
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach(GameObject component in components)
        {
            Vector3 position = component.transform.localPosition;
            position.x = -position.x;
            component.transform.localPosition = position;
        }
    }
}
