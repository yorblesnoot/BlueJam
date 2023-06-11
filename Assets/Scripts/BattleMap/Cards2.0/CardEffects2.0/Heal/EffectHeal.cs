using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDamage", menuName = "ScriptableObjects/CardEffects/Damage")]
public class EffectHeal : CardEffectPlus
{

    //GreenHeal, HealBurst

    public override string GenerateDescription()
    {
        description = "Heal for [heal]";
        return description;
    }
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        List<GameObject> targets = base.Execute(actor, targetCell, aoe);
        foreach (GameObject target in targets) Heal(actor, scalingMultiplier, target);
        return targets;
    }
    //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Heal(GameObject owner, float healMult, GameObject target)
    {
        int heal = -Mathf.RoundToInt(owner.GetComponent<BattleUnit>().healScaling * healMult);
        target.GetComponent<BattleUnit>().ReceiveDamage(heal);
    }
}
