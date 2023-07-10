using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipReader : MonoBehaviour
{
    ITooltipEnabled currentHighlight;
    int counterToTooltip;
    private void Awake()
    {
        StartCoroutine(MonitorMouseForTooltip());
    }
    IEnumerator MonitorMouseForTooltip()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                counterToTooltip = 0;
                continue;
            }
            PointerEventData pointer = new(EventSystem.current)
            { position = Input.mousePosition };

            List<RaycastResult> raycastResults = new();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            GameObject underMouse = raycastResults[0].gameObject;
            
            ITooltipEnabled tooltipCarrier = underMouse.GetComponent<ITooltipEnabled>();
            if (tooltipCarrier == null)
            {
                counterToTooltip = 0;
                continue;
            }
            if (currentHighlight == tooltipCarrier) counterToTooltip++;
            currentHighlight = tooltipCarrier;
            if (counterToTooltip >= 5) Debug.Log(tooltipCarrier.tooltipDescription);
        }

    }
}
