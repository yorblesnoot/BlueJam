using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldItem", menuName = "ScriptableObjects/Item/WorldItem")]
public class WorldItem : Item
{
    [SerializeField] List<ItemMod> mods;
    public void ActivateItem(Unit player)
    {
        foreach (var mod in mods)
        {
            mod.ModifyPlayer(player);
        }
    }
}
