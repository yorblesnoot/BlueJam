using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectPlus : ScriptableObject
{
    public bool forceTargetSelf;
    public bool targetNotRequired;

    public CardClass effectClass;

    public VFXStyle vfxStyleBefore;
    [StringInList(typeof(VFXHelper), "AllVFXNames")] public string vfxNameBefore;

    public VFXStyle vfxStyleAfter;
    [StringInList(typeof(VFXHelper), "AllVFXNames")] public string vfxNameAfter;

    public SoundTypeEffect effectSound;

    public float scalingMultiplier;

    [SerializeField] TileMapShape aoeShape;
    [SerializeField] int aoeSize;
    [SerializeField] int aoeGap;
    [HideInInspector] public bool[,] aoe;

    [HideInInspector] public BattleTileController userOriginalTile;

    public bool blockTrigger;

    public static readonly Dictionary<TileMapShape, string> aoeShapeName = new()
    {
        {TileMapShape.CROSS, "<color=blue>+</color>" },
        { TileMapShape.CIRCLE, "<color=blue>O</color>" },
        { TileMapShape.DIAGONALCROSS, "<color=blue>X</color>"},
        { TileMapShape.SQUARE, "<color=blue>[  ]</color>"}
    };
    public void Initialize()
    {
        aoe = MapRulesGenerator.Convert(aoeShape, aoeSize, aoeGap);
    }

    public IEnumerator Execute(BattleUnit actor, BattleTileController targetCell)
    {
        if (forceTargetSelf == true) targetCell = userOriginalTile;
        List<BattleUnit> targets = AcquireTargets(actor, targetCell, aoe);

        SoundManager.PlaySound(effectSound);

        VFXMachine.PlayVFX(vfxNameBefore, vfxStyleBefore, actor, targetCell);
        yield return targetCell.StartCoroutine(ActivateEffect(actor, targetCell, aoe, targets));
        VFXMachine.PlayVFX(vfxNameAfter, vfxStyleAfter, actor, targetCell);

        if (targets.Count > 0) foreach(BattleUnit target in targets) EventManager.checkForTriggers.Invoke(this, actor, target);
        else EventManager.checkForTriggers.Invoke(this, actor, null);
    }

    public List<BattleUnit> AcquireTargets(BattleUnit actor, BattleTileController targetCell, bool[,] aoe)
    {
        return CellTargeting.AreaTargets(targetCell.gameObject, actor.gameObject.tag, effectClass, aoe);
    }

    public virtual IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null) { yield return null; }

    public string GenerateDescription(IUnitStats unit)
    {
        string baseDescription = GetEffectDescription(unit);
        if(aoeSize > 0) baseDescription = AppendAOEInfo(baseDescription);
        if (forceTargetSelf) 
        {
            if (aoeSize > 0)
                baseDescription += " around user";
            else baseDescription += " on user";
        }
        return baseDescription;
    }

    internal string AppendAOEInfo(string description)
    {
        string output = description;
        output += $" in a <color=red>{aoeSize}{aoeShapeName[aoeShape]}</color> AOE";
        return output;
    }

    public virtual string GetEffectDescription(IUnitStats unit)
    {
        return "";
    }
}
