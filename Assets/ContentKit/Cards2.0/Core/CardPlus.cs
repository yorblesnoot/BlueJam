using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
public class CardPlus : SOWithGUID
{
    //core card parameters

    public string displayName;
    public int cost;
    [HideInInspector]public string description;
    [HideInInspector] public string keywords;

    [SerializeField]TileMapShape targetShape;
    [SerializeField] int targetSize;
    [SerializeField] int targetGap;
    [HideInInspector] public bool[,] targetRules;
    public bool pathCheckForTargets;

    [SerializeField] TileMapShape aoeShape;
    [SerializeField] int aoeSize;
    [SerializeField] int aoeGap;  
    [HideInInspector]public bool[,] aoeRules;

    public List<CardClass> cardClass;
    public bool consumed;

    public AnimType animType;

    public List<CardEffectPlus> effects;

    GameObject player;

    public void Initialize()
    {
        targetRules = MapRulesGenerator.Convert(targetShape, targetSize, targetGap);
        aoeRules = MapRulesGenerator.Convert(aoeShape, aoeSize, aoeGap);
        player = GameObject.FindGameObjectWithTag("Player");
        AssembleDescription();
    }

    public IEnumerator PlaySequence(BattleUnit actor, BattleTileController targetCell)
    {
        actor.transform.LookAt(targetCell.unitPosition);
        actor.unitAnimator.Animate(animType);
        BattleTileController userOriginalTile = GridTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>();
        EventManager.allowTriggers.Invoke();
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].userOriginalTile = userOriginalTile;
            effects[i].Execute(actor, targetCell, aoeRules);
            yield return new WaitUntil(() => effects[i].doneExecuting == true);
            //yield return new WaitForSeconds(effects[i].delayAfter);
        }
        TurnManager.SpendBeats(actor.GetComponent<BattleUnit>(), cost);
    }
    public void AssembleDescription()
    {
        description = "";
        //for the purposes of generating a description, the owner should always be the player
        keywords = "";
        IPlayerData stats = player.GetComponent<IPlayerData>();
        int ownerDamage = stats.damageScaling;
        int ownerHealing = stats.healScaling;
        int ownerBarrier = stats.barrierScaling;

        for (int i = 0; i < effects.Count; i++)
        {
            description += $"{effects[i].GenerateDescription(stats)}.";
            description += Environment.NewLine;
        }
        if (consumed == true)
        {
            keywords += "~Consumed~";
        }
    }
}