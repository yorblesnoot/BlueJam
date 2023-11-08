using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : SOWithGUID
{
    public Sprite thumbnail;
    public Color32 thumbnailColor;
    [SerializeField] string description;
    private void Reset()
    {
        thumbnailColor = Color.white;
    }

    public string GetDescription()
    {
        return $"<b><i>{name.SplitCamelCase()}:</i></b> {description}";
    }
    
    public virtual void PlayerGetItem(RunData runData)
    {
        runData.itemInventory.Add(this);
        EventManager.updateItemUI.Invoke();
    }
}
