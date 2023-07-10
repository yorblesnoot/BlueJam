using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [HideInInspector] public BattleItem battleItem;

    public Image thumbnail;

    public string tooltipDescription { get; set; }

    public virtual void DisplayItem(BattleItem item)
    {
        battleItem = item;
        tooltipDescription = item.description;
        thumbnail.sprite = item.thumbnail;
        thumbnail.color = item.thumbnailColor;
    }
}
