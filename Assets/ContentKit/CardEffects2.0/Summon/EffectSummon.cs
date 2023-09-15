using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectSummon", menuName = "ScriptableObjects/CardEffects/Summon")]
public class EffectSummon : CardEffectPlus
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.SUMMON;
        effectClass = CardClass.SUMMON;
    }
    public GameObject entityToSummon;
    public override string GetEffectDescription(Unit player)
    {
        string entityName = entityToSummon.name;
        entityName = entityName.Replace("NPC", "");
        return $"summon a{(entityName.CharacterIsVowel(0) ? "n" : "")} <color=orange>{entityName}</color>";
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
        summoned.transform.LookAt(new Vector3(actor.transform.position.x, summoned.transform.position.y ,actor.transform.position.z));
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
            toModify.transform.Find("PlayerAllyHalo").gameObject.SetActive(true);
        }
        else toModify.tag = owner.tag;
        toModify.transform.localScale -= new Vector3(.3f, .3f, .3f);

        toModify.isSummoned = true;
        toModify.loadedStats[StatType.MAXHEALTH] *= summonModifier;
        toModify.currentHealth = Mathf.RoundToInt(toModify.loadedStats[StatType.MAXHEALTH]);
        toModify.GetComponentInChildren<NonplayerUI>().InitializeHealth();

        toModify.loadedStats[StatType.DAMAGE] *= summonModifier;
        toModify.loadedStats[StatType.HEAL] *= summonModifier;
        toModify.loadedStats[StatType.BARRIER] *= summonModifier;

        toModify.RegisterTurn();
        toModify.ReportCell();
    }
}
