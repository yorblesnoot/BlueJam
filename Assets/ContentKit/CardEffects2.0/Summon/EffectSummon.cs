using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EffectDamage;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "EffectSummon", menuName = "ScriptableObjects/CardEffects/Summon")]
public class EffectSummon : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.SUMMON;
    }
    public GameObject entityToSummon;
    public override string GenerateDescription(IPlayerStats player)
    {
        string entityName = entityToSummon.name;
        entityName = entityName.Replace("NPC", "");
        return $"summon a {entityName}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        Vector3 location = new();
        if (aoe.GetLength(0) > 1)
        {
            List<GameObject> cells = CellTargeting.ConvertMapRuleToTiles(aoe, targetCell.transform.position);
            while (cells.Count > 0)
            {
                int cellIndex = Random.Range(0, cells.Count);
                BattleTileController cell = cells[cellIndex].GetComponent<BattleTileController>();
                if (CellTargeting.TileIsValidTarget(cell, actor.gameObject.tag, CardClass.SUMMON))
                {
                    location = cell.GetComponent<BattleTileController>().unitPosition;
                    break;
                }
                cells.RemoveAt(cellIndex);
            }
        }
        else location = targetCell.unitPosition;
        GameObject summoned = Instantiate(entityToSummon, location, Quaternion.identity);
        summoned.transform.LookAt(actor.transform);
        ModifyStats(actor, summoned.GetComponent<NonplayerUnit>());
        NonplayerHandPlus hand = summoned.GetComponent<NonplayerHandPlus>();
        hand.BuildVisualDeck();
        hand.DrawPhase();
        yield return null;
    }

    void ModifyStats(BattleUnit owner, NonplayerUnit toModify)
    {
        float summonModifier = .6f;
        if(owner.CompareTag("Player"))
        {
            toModify.tag = "Ally";
        }
        else toModify.tag = owner.tag;
        toModify.transform.localScale -= new Vector3(.3f, .3f, .3f);

        toModify.isSummoned = true;
        toModify.maxHealth = Mathf.RoundToInt(toModify.maxHealth * summonModifier);
        toModify.currentHealth = toModify.maxHealth;

        toModify.DamageScaling = Mathf.RoundToInt(toModify.DamageScaling * summonModifier);
        toModify.healScaling = Mathf.RoundToInt(toModify.healScaling * summonModifier);
        toModify.barrierScaling = Mathf.RoundToInt(toModify.barrierScaling * summonModifier);

        toModify.RegisterTurn();
        toModify.ReportCell();
    }
}
