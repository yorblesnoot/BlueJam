using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPool", menuName = "ScriptableObjects/Singletons/ItemPool")]
public class ItemPool : ScriptableObject
{
    [field:SerializeField] public List<BattleItem> awardableItemPool { get; private set; }

    [HideInInspector] public List<BattleItem> awardableItems;
}
