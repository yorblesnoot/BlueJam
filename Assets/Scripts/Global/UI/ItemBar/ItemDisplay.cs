using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [HideInInspector] public Item battleItem;
    public Image thumbnail;

    public virtual void DisplayItem(Item item)
    {
        battleItem = item;
        thumbnail.sprite = item.thumbnail;
        thumbnail.color = item.thumbnailColor;
    }
}
