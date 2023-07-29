using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : SOWithGUID
{
    public Sprite thumbnail;
    public Color32 thumbnailColor;
    public string description;
    private void Reset()
    {
        thumbnailColor = Color.white;
    }
    
    public virtual void PlayerGetItem(RunData runData)
    {
        runData.itemInventory.Add(this);
        EventManager.updateItemUI.Invoke();
    }
}
