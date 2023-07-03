using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [HideInInspector]public bool[,] aoePoint;
    [HideInInspector]public bool[,] aoeSelf;

    [HideInInspector] public List<CardClass> cardClass;
    public bool consumed;

    public AnimType animType;

    public List<CardEffectPlus> effects;

    GameObject player;

    public void Initialize()
    {
        targetRules = MapRulesGenerator.Convert(targetShape, targetSize, targetGap);

        player = GameObject.FindGameObjectWithTag("Player");
        foreach (CardEffectPlus effect in effects)
        {
            CardClass cclass = effect.effectClass;
            effect.Initialize();
            if(!cardClass.Contains(cclass)) cardClass.Add(cclass);
        }

        List<bool[,]> selfs = new();
        List<bool[,]> points = new();
        foreach (var effect in effects)
        {
            if(effect.targetSelf == true) selfs.Add(effect.aoe);
            else points.Add(effect.aoe);
        }
        if(selfs.Count > 0) aoeSelf = CellTargeting.CombineAOEIndicators(selfs);
        aoePoint = CellTargeting.CombineAOEIndicators(points);

        AssembleDescription();
    }

    public IEnumerator PlaySequence(BattleUnit actor, BattleTileController targetCell)
    {
        actor.transform.LookAt(targetCell.unitPosition);
        actor.unitAnimator.Animate(animType);
        BattleTileController userOriginalTile = MapTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>();
        EventManager.allowTriggers.Invoke();
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].userOriginalTile = userOriginalTile;
            effects[i].Execute(actor, targetCell);
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
        IPlayerStats stats = player.GetComponent<IPlayerStats>();
        for (int i = 0; i < effects.Count; i++)
        {
            description += $"{effects[i].GenerateDescription(stats)}.";
            description += Environment.NewLine;
        }
        keywords += $"Range: {targetGap}-{targetSize} ";
        keywords += $"AOE: {aoePoint.GetLength(0)-1} ";
        if (consumed == true)
        {
            keywords += "~Consumed~";
        }
    }
}