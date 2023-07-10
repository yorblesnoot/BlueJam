using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CardEffectPlus : ScriptableObject
{
    public bool forceTargetSelf;

    public CardClass effectClass;

    public VFXStyle vfxStyleSelf;
    [StringInList(typeof(VFXHelper), "AllVFXNames")] public string vfxNameSelf;

    public VFXStyle vfxStyleTarget;
    [StringInList(typeof(VFXHelper), "AllVFXNames")] public string vfxNameTarget;

    public float scalingMultiplier;

    [SerializeField] TileMapShape aoeShape;
    [SerializeField] int aoeSize;
    [SerializeField] int aoeGap;
    [HideInInspector] public bool[,] aoe;

    [HideInInspector] public BattleTileController userOriginalTile;

    public bool blockTrigger;

    public void Initialize()
    {
        aoe = MapRulesGenerator.Convert(aoeShape, aoeSize, aoeGap);
    }

    public IEnumerator Execute(BattleUnit actor, BattleTileController targetCell)
    {
        List<BattleUnit> targets = AcquireTargets(actor, targetCell, aoe);

        VFXMachine.PlayVFX(vfxNameSelf, vfxStyleSelf, actor, MapTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>());
        yield return actor.StartCoroutine(ActivateEffect(actor, targetCell, aoe, targets));
        VFXMachine.PlayVFX(vfxNameTarget, vfxStyleTarget, actor, targetCell);

        if (targets.Count > 0) foreach(BattleUnit target in targets) EventManager.checkForTriggers.Invoke(this, actor, target);
        else EventManager.checkForTriggers.Invoke(this, actor, null);
    }

    public List<BattleUnit> AcquireTargets(BattleUnit actor, BattleTileController targetCell, bool[,] aoe)
    {
        if (forceTargetSelf == true) targetCell = userOriginalTile;
        return CellTargeting.AreaTargets(targetCell.gameObject, actor.gameObject.tag, effectClass, aoe);
    }

    public virtual IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null) { yield return null; }

    public virtual string GenerateDescription(IPlayerStats player)
    {
        return "";
    }
}
