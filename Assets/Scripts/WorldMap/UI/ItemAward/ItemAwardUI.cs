using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAwardUI : MonoBehaviour
{
    [SerializeField] ItemPool pool;
    List<Item> awardedItems;

    [SerializeField] List<AwardedItemOption> awardSlots;

    [SerializeField] RunData runData;

    [SerializeField] GameObject awardScreen;

    private void Awake()
    {
        EventManager.awardItem.AddListener(ShowAwardableItems);
    }
    public void ShowAwardableItems()
    {
        awardScreen.SetActive(true);
        awardedItems = new();
        foreach (var item in awardSlots)
        {
            //pull random items from the award list for each slot
            if(pool.awardableItems.Count > 0)
            { 
                int awardIndex = Random.Range(0, pool.awardableItems.Count);
                item.DisplayItem(pool.awardableItems[awardIndex]);

                //move the displayed items into a different list
                awardedItems.Add(pool.awardableItems[awardIndex]);
                pool.awardableItems.RemoveAt(awardIndex);
            }
            else
            {
                item.DisplayItem(null);
                break;
            }
        }
    }

    public void AwardItem(Item award)
    {
        //return the unselected items to the pool and give the player the selected item
        runData.itemInventory.Add(award);
        awardedItems.Remove(award);
        pool.awardableItems.AddRange(awardedItems);
        EventManager.updateItemUI.Invoke();
        awardScreen.SetActive(false);
    }
}
