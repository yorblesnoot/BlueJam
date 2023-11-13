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
    public Sprite art;
    public int cost;
    [HideInInspector] public string description;
    [HideInInspector] public string keywords;

    [SerializeField]TileMapShape targetShape;
    [SerializeField] int targetSize;
    [SerializeField] int targetGap;
    [HideInInspector] public bool[,] targetRules;
    public bool needsPath;
    public bool isCurse;

    [HideInInspector] public bool[,] aoePoint;
    [HideInInspector] public bool[,] aoeSelf;

    public bool consumed;

    public AnimType animType;
    [SerializeField] float delayBeforeEffects;

    public List<CardEffectPlus> effects;

    static Unit player;

    public void Initialize()
    {
        targetRules = MapRulesGenerator.Convert(targetShape, targetSize, targetGap);
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();

        List<bool[,]> selfs = new();
        List<bool[,]> points = new();
        foreach (var effect in effects)
        {
            effect.Initialize();
            if (effect.forceTargetSelf == true) selfs.Add(effect.aoe);
            else points.Add(effect.aoe);
        }
        if(selfs.Count > 0) aoeSelf = CellTargeting.CombineAOEIndicators(selfs);
        aoePoint = CellTargeting.CombineAOEIndicators(points);

        AssembleDescription();
    }

    public IEnumerator PlaySequence(BattleUnit actor, BattleTileController targetCell)
    {
        actor.SpendBeats(cost);
        actor.transform.LookAt(new Vector3(targetCell.unitPosition.x, actor.transform.position.y, targetCell.unitPosition.z));
        actor.unitAnimator.Animate(animType);
        yield return new WaitForSeconds(delayBeforeEffects);
        BattleTileController userOriginalTile = MapTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>();
        EventManager.allowTriggers.Invoke();
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].userOriginalTile = userOriginalTile;
            yield return actor.StartCoroutine(effects[i].Execute(actor, targetCell));          
        }
        actor.lastPlayed = this;
        TurnManager.FinalizeTurn(actor);
    }
    public void AssembleDescription()
    {
        description = "";
        //for the purposes of generating a description, the owner should always be the player
        keywords = "";
        for (int i = 0; i < effects.Count; i++)
        {
            string effectDescription = effects[i].GenerateDescription(player);
            if (effectDescription == "") continue;
            description += $"{effectDescription.FirstToUpper()}.";
            description += " ";
            //Environment.NewLine
        }
        keywords += $"Range: {targetGap}-{targetSize} {CardEffectPlus.aoeShapeName[targetShape]}.";
        if (consumed) keywords += " <color=red>Consumed.</color>";
        if (needsPath) keywords += " Requires LoS.";
    }
}