using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WrldCardDisplay : PlayerCardDisplay, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // award or remove the card
        EventManager.clickedCard.Invoke(thisCard, gameObject);
    }
}
