using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Cards/Core")]
public class CardPlus : ScriptableObject
{
    //core card parameters

    public string displayName;
    public int cost;
    [HideInInspector]public string description;
    [HideInInspector] public string keywords;

    [SerializeField]TileMapShape targetShape;
    [SerializeField] int targetSize;
    [SerializeField] int targetGap;
    [HideInInspector] public string[,] targetRules;
    public bool pathCheckForTargets;

    [SerializeField] TileMapShape aoeShape;
    [SerializeField] int aoeSize;
    [SerializeField] int aoeGap;  
    [HideInInspector]public string[,] aoeRules;

    public List<CardClass> cardClass;
    public bool consumed;

    public List<CardEffectPlus> effects;

    public void Initialize()
    {
        targetRules = MapRulesGenerator.Convert(targetShape, targetSize, targetGap);
        aoeRules = MapRulesGenerator.Convert(aoeShape, aoeSize, aoeGap);
        AssembleDescription();
    }

    public IEnumerator PlaySequence(BattleUnit actor, BattleTileController targetCell)
    {
        BattleTileController userOriginalTile = GridTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>();
        EventManager.allowTriggers.Invoke();
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].userOriginalTile = userOriginalTile;
            effects[i].Execute(actor, targetCell, aoeRules);
            yield return new WaitForSeconds(effects[i].delayAfter);
        }
        TurnManager.SpendBeats(actor.GetComponent<BattleUnit>(), cost);
    }
    public void AssembleDescription()
    {
        description = "";
        //for the purposes of generating a description, the owner should always be the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        keywords = "";
        IPlayerData stats = player.GetComponent<IPlayerData>();
        int ownerDamage = stats.damageScaling;
        int ownerHealing = stats.healScaling;
        int ownerBarrier = stats.barrierScaling;

        List<ScalingType> scalingTypes = new List<ScalingType>();
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].GenerateDescription();
            description += $"{effects[i].description}.";
            description = description.Replace("[damage]", "<color=#FF4E2B>" + (ownerDamage * effects[i].scalingMultiplier).ToString() + "</color>");
            description = description.Replace("[barrier]", "<color=#1ED5FA>" + (ownerBarrier * effects[i].scalingMultiplier).ToString() + "</color>");
            description = description.Replace("[heal]", "<color=#1EFA61>" + (ownerHealing * effects[i].scalingMultiplier).ToString() + "</color>");
            description += Environment.NewLine;
        }
        if (consumed == true)
        {
            keywords += "~Consumed~";
        }
    }
}