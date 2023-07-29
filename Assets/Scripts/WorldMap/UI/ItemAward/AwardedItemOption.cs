using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AwardedItemOption : ItemDisplay
{
    [SerializeField] TMP_Text description;

    [SerializeField] ItemAwardUI awarder;

    public void Clicked()
    {
        if(battleItem != null) awarder.AwardItem(battleItem);
    }

    public override void DisplayItem(Item item)
    {
        if (item == null)
        {
            description.text = "There are no other items to award! :(";
            return;
        }
        base.DisplayItem(item);
        description.text = item.description;
    }
}
