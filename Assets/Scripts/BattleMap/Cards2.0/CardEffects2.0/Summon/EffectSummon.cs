using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EffectDamage;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "EffectSummon", menuName = "ScriptableObjects/CardEffects/Summon")]
public class EffectSummon : CardEffectPlus
{
    public GameObject entityToSummon;
    public override string GenerateDescription()
    {
        description = $"Summons a {entityToSummon.name}";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        Vector3 location = targetCell.GetComponent<BattleTileController>().unitPosition;
        GameObject summoned = Instantiate(entityToSummon, location, Quaternion.identity);
        ModifyStats(actor, summoned.GetComponent<BattleUnit>());
        VFXMachine.PlayAtLocation("SummonCircles", location);
        return null;
    }

    void ModifyStats(BattleUnit owner, BattleUnit toModify)
    {
        int summonModifier = 2;
        if(owner.tag == "Player")
        {
            toModify.tag = "Ally";
        }
        else toModify.tag = owner.tag;
        toModify.transform.localScale -= new Vector3(.3f, .3f, .3f);

        toModify.isSummoned = true;
        toModify.maxHealth /= summonModifier;
        toModify.currentHealth /= summonModifier;

        toModify.damageScaling /= summonModifier;
        toModify.healScaling /= summonModifier;
        toModify.barrierScaling /= summonModifier;

        toModify.RegisterTurn();
        toModify.ReportCell();
    }
}
