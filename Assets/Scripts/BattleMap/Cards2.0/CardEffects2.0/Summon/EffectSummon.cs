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
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        Vector3 location = targetCell.GetComponent<BattleTileController>().unitPosition;
        GameObject summoned = GameObject.Instantiate(entityToSummon, location, Quaternion.identity);
        ModifyStats(actor, summoned);
        VFXMachine.PlayAtLocation("SummonCircles", location);
        return null;
    }

    void ModifyStats(GameObject owner, GameObject toModify)
    {
        int summonModifier = 2;
        if(owner.tag == "Player")
        {
            toModify.tag = "Ally";
        }
        else toModify.tag = owner.tag;
        toModify.transform.localScale -= new Vector3(.3f, .3f, .3f);

        BattleUnit stats = toModify.GetComponent<BattleUnit>();
        stats.isSummoned = true;
        stats.maxHealth /= summonModifier;
        stats.currentHealth /= summonModifier;

        stats.damageScaling /= summonModifier;
        stats.healScaling /= summonModifier;
        stats.barrierScaling /= summonModifier;

        stats.RegisterTurn();
        stats.ReportCell();
    }
}
