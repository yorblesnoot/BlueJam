using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldItem", menuName = "ScriptableObjects/Item/WorldItem")]
public class WorldItem : Item
{
    [SerializeField] List<ItemMod> mods;
    public override void PlayerGetItem(RunData runData)
    {
        base.PlayerGetItem(runData);
        foreach (var mod in mods)
        {
            mod.ModifyPlayer(runData);
        }
    }
}
