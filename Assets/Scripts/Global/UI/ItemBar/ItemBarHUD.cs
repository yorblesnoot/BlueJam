using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBarHUD : MonoBehaviour
{
    [SerializeField] List <ItemDisplay> battleItems;
    [SerializeField] RunData runData;
    private void Awake()
    {
        DisplayItems();
        EventManager.updateItemUI.AddListener(DisplayItems);
    }

    public void DisplayItems()
    {
        foreach(var display in battleItems)
        {
            display.gameObject.SetActive(false);
        }
        for (int i = 0; i < runData.itemInventory.Count; i++)
        {
            battleItems[i].gameObject.SetActive(true);
            battleItems[i].DisplayItem(runData.itemInventory[i]);
        }
    }
}
