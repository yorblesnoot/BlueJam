using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BattleItem", menuName = "ScriptableObjects/Item/BattleItem")]
public class BattleItem : Item
{
    public List<CardEffectPlus> effects;
}
