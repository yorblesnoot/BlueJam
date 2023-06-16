using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CardEffectPlus : ScriptableObject
{
    public float delayAfter;
    public bool targetSelf;

    public CardClass effectClass;

    public VFXStyle vfxStyle;
    public string vfxName;

    public float scalingMultiplier;

    [HideInInspector] public string description;

    [HideInInspector] public BattleTileController userOriginalTile;

    public bool blockTrigger;
    public virtual List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        if(vfxName != null && vfxName != "") VFXMachine.PlayVFX(vfxName, vfxStyle, actor, targetCell);
        if(targetSelf == true)
        {
            targetCell = userOriginalTile;
        }
        //issue here when no targets
        List<BattleUnit> targets = CellTargeting.AreaTargets(targetCell.gameObject, actor.gameObject.tag, effectClass, aoe);
        if(targets.Count > 0) foreach(BattleUnit target in targets) EventManager.checkForTriggers.Invoke(this, actor, target);
        else EventManager.checkForTriggers.Invoke(this, actor, null);
        return targets;
    }

    public virtual string GenerateDescription()
    {
        return description;
    }
}
