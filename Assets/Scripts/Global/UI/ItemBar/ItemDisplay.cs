using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [HideInInspector] public BattleItem battleItem;
    public Image thumbnail;

    public virtual void DisplayItem(BattleItem item)
    {
        battleItem = item;
        thumbnail.sprite = item.thumbnail;
        thumbnail.color = item.thumbnailColor;
    }
}
