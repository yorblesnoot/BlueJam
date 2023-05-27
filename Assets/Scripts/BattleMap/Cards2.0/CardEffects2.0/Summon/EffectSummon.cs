using UnityEditor;
using UnityEngine;
using static EffectDamageHeal;

[CreateAssetMenu(fileName = "EffectSummon", menuName = "ScriptableObjects/CardEffects/Summon")]
public class EffectSummon : CardEffectPlus
{
    public GameObject entityToSummon;
    public override string GenerateDescription()
    {
        description = $"Summons a {entityToSummon.name}";
        return description;
    }
    public override void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        Spawn(actor, entityToSummon, targetCell);
    }
    void Spawn(GameObject owner, GameObject entity, GameObject tile)
    {
        Vector3 location = tile.GetComponent<BattleTileController>().unitPosition;
        GameObject summoned = GameObject.Instantiate(entity, location, Quaternion.identity);
        ModifyStats(owner, summoned);
        VFXMachine.PlayAtLocation("SummonCircles", location);
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

        UnitActions stats = toModify.GetComponent<UnitActions>();
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
