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
}
