using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDamage", menuName = "ScriptableObjects/CardEffects/Damage")]
public class EffectDamageHeal : CardEffectPlus
{
    public enum DamageType { DAMAGE, HEAL}
    public DamageType damageType;

    //ImpactSmall, GroundBurst, GreenHeal, HealBurst

    public override string GenerateDescription()
    {
        if (damageType == DamageType.DAMAGE)
            description = "Deal [damage] damage";
        else if (damageType == DamageType.HEAL)
            description = "Heal for [heal]";
        return description;
    }
    public override void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        MonoBehaviour unitStats = actor.GetComponent<UnitActions>();
        if (damageType == DamageType.DAMAGE) Normal(actor, scalingMultiplier, targetCell, aoe);
        else if (damageType == DamageType.HEAL) Heal(actor, scalingMultiplier, targetCell, aoe);
    }
        //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Normal(GameObject owner, float damageMult, GameObject targetCell, string[,] aoe)
    {
        int damage = Mathf.RoundToInt(owner.GetComponent<UnitActions>().damageScaling * damageMult);
        List<GameObject> toDamage = ZoneTargeter.AreaTargets(targetCell, owner.tag, CardClass.ATTACK, aoe);
        for(int i = 0; i < toDamage.Count; i++) 
        {
            toDamage[i].GetComponent<UnitActions>().ReceiveDamage(damage);
        }
    }
    void Heal(GameObject owner, float healMult, GameObject targetCell, string[,] aoe)
    {
        int heal = -Mathf.RoundToInt(owner.GetComponent<UnitActions>().healScaling * healMult);
        List<GameObject> toHeal = ZoneTargeter.AreaTargets(targetCell, owner.tag, CardClass.BUFF, aoe);
        for (int i = 0; i < toHeal.Count; i++)
        {
            toHeal[i].GetComponent<UnitActions>().ReceiveDamage(heal);
        }
    }
}
