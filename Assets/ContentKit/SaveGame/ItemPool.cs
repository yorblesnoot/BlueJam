using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPool", menuName = "ScriptableObjects/Singletons/ItemPool")]
public class ItemPool : ScriptableObject
{
    [field:SerializeField] public List<Item> awardableItemPool { get; private set; }

    [HideInInspector] public List<Item> awardableItems;
}
