using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BattleItem", menuName = "ScriptableObjects/Item/BattleItem")]
public class BattleItem : SOWithGUID
{
    public Sprite thumbnail;
    public Color32 thumbnailColor;

    public List<CardEffectPlus> effects;

    public string description;

    private void Reset()
    {
        thumbnailColor = Color.white;
    }
}
