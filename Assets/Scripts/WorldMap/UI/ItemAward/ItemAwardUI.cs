using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAwardUI : MonoBehaviour
{
    [SerializeField]ItemPool pool;
    List<BattleItem> awardedItems;

    public List<AwardedItemOption> awardSlots;

    [SerializeField] RunData runData;

    [SerializeField] GameObject menu;

    private void Awake()
    {
        EventManager.awardItem.AddListener(ShowAwardableItems);
    }
    public void ShowAwardableItems()
    {
        menu.SetActive(true);
        awardedItems = new();
        foreach (var item in awardSlots)
        {

            //pull random items from the award list for each slot
            int awardIndex = Random.Range(0, pool.awardableItems.Count);
            try
            {
                item.DisplayItem(pool.awardableItems[awardIndex]);

                //move the displayed items into a different list
                awardedItems.Add(pool.awardableItems[awardIndex]);
                pool.awardableItems.RemoveAt(awardIndex);
            }
            catch
            {
                item.DisplayItem(null);
                break;
            }
        }
    }

    public void AwardItem(BattleItem award)
    {
        //return the unselected items to the pool and give the player the selected item
        runData.itemInventory.Add(award);
        awardedItems.Remove(award);
        pool.awardableItems.AddRange(awardedItems);
        EventManager.updateItemUI.Invoke();
        menu.SetActive(false);
    }
}
