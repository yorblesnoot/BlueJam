using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDamage", menuName = "ScriptableObjects/CardEffects/Damage")]
public class EffectDamage : CardEffectPlus
{

    //ImpactSmall, GroundBurst

    public override string GenerateDescription()
    {
        description = "Deal [damage] damage";
        return description;
    }
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        List<GameObject> targets = base.Execute(actor, targetCell, aoe);
        foreach (GameObject target in targets) Normal(actor, scalingMultiplier, target);
        return targets;
    }
        //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Normal(GameObject owner, float damageMult, GameObject target)
    {
        int damage = Mathf.RoundToInt(owner.GetComponent<BattleUnit>().damageScaling * damageMult);
        target.GetComponent<BattleUnit>().ReceiveDamage(damage);
    }
}
