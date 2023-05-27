using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public virtual void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        VFXMachine.PlayVFX(vfxName, vfxStyle, actor, targetCell);
        if(targetSelf == true)
        {
            targetCell = userOriginalTile;
        }
    }

    public virtual string GenerateDescription()
    {
        return description;
    }
}
