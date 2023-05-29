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

    [HideInInspector] public GameObject userOriginalTile;

    public bool blockTrigger;
    public virtual List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        VFXMachine.PlayVFX(vfxName, vfxStyle, actor, targetCell);
        if(targetSelf == true)
        {
            targetCell = userOriginalTile;
        }
        List<GameObject> targets = ZoneTargeter.AreaTargets(targetCell, actor.tag, effectClass, aoe);
        if(targets.Count > 0) foreach(GameObject target in targets) EventManager.checkForTriggers.Invoke(this, actor, target);
        else EventManager.checkForTriggers.Invoke(this, actor, null);
        return targets;
    }

    public virtual string GenerateDescription()
    {
        return description;
    }
}
