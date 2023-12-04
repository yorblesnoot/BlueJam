using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Data;

public class UnitAI : MonoBehaviour
{
    public AIProfile personality;

    private List<GameObject> entities;

    [SerializeField] HandPlus myHand;
    [SerializeField] BattleUnit thisUnit;

    Pathfinder pathfinder;

    //get information from cards: legal targets, card class, target requirements, object reference
    private void OnEnable()
    {
        pathfinder = new(false);
    }

    struct PossiblePlay
    {
        public ICardDisplay card;
        public BattleTileController target;
        public float favor;
    }

    public void AITakeTurn()
    {
        //1. put all possible moves with their respective cards in one place
        //checklegal on every targetRules
        entities = TurnManager.turnTakers.Where(t => t.Allegiance != AllegianceType.ALLY && t.GetType() == typeof(NonplayerUnit)).Select(t => t.gameObject).ToList();
        entities.Add(TurnManager.playerUnit.gameObject);
        entities.Remove(gameObject);

        List<PossiblePlay> possiblePlays = new();
        for(int rule = 0; rule < myHand.handCards.Count; rule++)
        {
            //use battlemap to find legal cells for every card in hand
            List<GameObject> legalTiles = CellTargeting.ConvertMapRuleToTiles(myHand.handCards[rule].thisCard.targetRules, transform.position);

            if (myHand.handCards[rule].thisCard.needsPath == true) legalTiles = legalTiles.EliminateUnpathable(gameObject);

            int ruleLength = legalTiles.Count;
            for(int x = 0; x < ruleLength; x++)
            {
                BattleTileController addTile = legalTiles[x].GetComponent<BattleTileController>();
                if (CellTargeting.ValidPlay(addTile, thisUnit, myHand.handCards[rule].thisCard))
                { 
                    possiblePlays.Add(new PossiblePlay
                    {
                        card = myHand.handCards[rule],
                        target = addTile,
                        favor = CalculateFavor(addTile, myHand.handCards[rule].thisCard),
                    });
                }
            }
        }
        //highest Favor means choose that option
        if(possiblePlays.Count > 0)
        {
            possiblePlays = possiblePlays.OrderByDescending(x => x.favor).ToList();
            //Debug.Log("playing " + possiblePlays[0].card.thisCard.displayName + " with " + possiblePlays[0].favor);
            StartCoroutine(AIPlayCard(possiblePlays[0].card, possiblePlays[0].target));
        }
        else
        {
            myHand.DiscardAll();
        }
    }

    IEnumerator AIPlayCard(ICardDisplay cardReference, BattleTileController targetTile)
    {
        ShowAITargeting(cardReference.thisCard.targetRules, transform.position, cardReference.thisCard);
        yield return new WaitForSeconds(Settings.Gameplay[GameplaySetting.NPC_cast_preview]);
        ShowAITargeting(cardReference.thisCard.aoePoint, targetTile.transform.position);
        if(cardReference.thisCard.aoeSelf != null) ShowAITargeting(cardReference.thisCard.aoeSelf, transform.position);
        yield return new WaitForSeconds(Settings.Gameplay[GameplaySetting.NPC_cast_preview]);

        //clear the range display and take the action
        EventManager.clearActivation.Invoke();
        StartCoroutine(myHand.DiscardCard(cardReference, true));
        StartCoroutine(cardReference.thisCard.PlaySequence(thisUnit,targetTile));
        CardHistory.AddCardToHistory(cardReference.thisCard, thisUnit);
    }

    void ShowAITargeting(bool[,] targetRule, Vector3 source, CardPlus card = null)
    {
        List<GameObject> displayCells = CellTargeting.ConvertMapRuleToTiles(targetRule, source);
        for (int i = 0; i < displayCells.Count; i++)
        {
            BattleTileController cellController = displayCells[i].GetComponent<BattleTileController>();
            if(card != null)
                cellController.HighlightCell(thisUnit, card);
            else
                cellController.HighlightCellAOE();
        }
    }

    private float CalculateFavor(BattleTileController moveTile, CardPlus card)
    {
        float favor = 0f;
        Vector2Int moveVect = MapTools.VectorToMap(moveTile.transform.position);
        foreach (CardEffectPlus effect in card.effects)
        {
            if (effect.effectClass == CardClass.MOVE)
            {
                favor += EntityScan(moveVect, personality.interestHostile, personality.interestFriendly, personality.proximityHostile, personality.proximityFriendly);
            }
            else if (effect.effectClass == CardClass.SUMMON)
            {
                favor += EntityScan(moveVect, personality.interestHostile, personality.interestFriendly, personality.summonProximityHostile, personality.summonProximityFriendly);
                favor += personality.interestSummon;
            }
            else
            {
                List<BattleUnit> targetables = CellTargeting.AreaTargets(effect.forceTargetSelf ? MapTools.VectorToTile(transform.position) : moveTile.gameObject,
                                                                         thisUnit.Allegiance,
                                                                         effect.effectClass,
                                                                         effect.aoe).Select(x => x.unitContents).ToList();
                if (targetables.FirstOrDefault(x => x.Allegiance == AllegianceType.PLAYER) != null) favor += .1f;

                if (effect.effectClass == CardClass.ATTACK)
                {
                    //hack for delayed effects; if we can't find a target, put it as close to an enemy as possible
                    if(targetables.Count == 0 && effect.GetType() == typeof(EffectDelayed))
                    {
                        //weighting is minimal so near-delays are only favored compared to eachother
                        favor += EntityScan(moveVect, personality.interestHostile, 0, 0, 0)/100000;
                    }
                    else favor += targetables.Count * personality.interestAttack;
                }
                else if (effect.effectClass == CardClass.BUFF)
                {
                    if(effect.GetType() != typeof(EffectHeal))
                    {
                        favor += targetables.Count * personality.interestBuff;
                        continue;
                    }
                    foreach (var target in targetables)
                    {
                        if (target.currentHealth < target.loadedStats[StatType.MAXHEALTH]) favor += personality.interestBuff;
                        else favor--;
                    }
                }
                    
            }
        }
        //Debug.Log($"{gameObject.name}: Favor for targeting {moveTile.transform.position} with {card.name} is {favor}.");
        return favor;
    }

    private bool WeAreFriends(GameObject meo, GameObject youo)
    {
        BattleUnit me = meo.GetComponent<BattleUnit>();
        BattleUnit you = youo.GetComponent<BattleUnit>();
        return FactionLogic.CheckIfFriendly(me.Allegiance, you.Allegiance);
    }

    private float DistanceProcessing(float desiredProximity, float movePathDistance, float currentPathDistance)
    {
        //divide by current path distance to make distant enemies less interesting when determining move priority
        return Mathf.Abs(desiredProximity - currentPathDistance) - Mathf.Abs(desiredProximity - movePathDistance)/currentPathDistance;
    }

    private float EntityScan(Vector2Int moveVect, float interestHostile, float interestFriendly, float proximityHostile, float proximityFriendly)
    {
        int friendlyCount = 0;
        float friendlyScore = 0;
        int hostileCount = 0;
        float hostileScore = 0;
        for (int i = 0; i < entities.Count; i++)
        {
            GameObject entity = entities[i];
            
            float potentialPathDistance = pathfinder.GetPathLength(moveVect, entity.transform.position.VectorToMap());
            float currentPathDistance = pathfinder.GetPathLength(transform.position.VectorToMap(), entity.transform.position.VectorToMap());
            if (potentialPathDistance == -1 || currentPathDistance == -1) continue;
            if (!WeAreFriends(gameObject, entity))
            {
                hostileScore += interestHostile * DistanceProcessing(proximityHostile, potentialPathDistance, currentPathDistance);
                hostileCount++;
            }
            else
            {
                friendlyScore += interestFriendly * DistanceProcessing(proximityFriendly, potentialPathDistance, currentPathDistance);
                friendlyCount++;
            }
        }
        float output = 0;
        if (hostileCount > 0) output += (hostileScore / hostileCount);
        if (friendlyCount > 0) output += friendlyScore / friendlyCount;
        return output;
    }
}

