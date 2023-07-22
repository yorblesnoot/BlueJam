using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Data;

public class UnitAI : MonoBehaviour
{
    [SerializeField] AIProfile personality;

    private List<BattleUnit> entities;

    [SerializeField] HandPlus myHand;
    [SerializeField] BattleUnit thisUnit;

    readonly float playDelay = .3f;

    Pathfinder pathfinder;

    //get information from cards: legal targets, card class, target requirements, object reference
    private void OnEnable()
    {
        pathfinder = new();
    }

    class PossiblePlay
    {
        public ICardDisplay card;
        public BattleTileController target;
        public float favor;
    }

    public void AITakeTurn()
    {
        //1. put all possible moves with their respective cards in one place
        //checklegal on every targetRules
        entities = new(TurnManager.turnTakers);
        entities = entities.Where(t => !(t.isSummoned && t.CompareTag("Ally"))).ToList();
        entities.Add(TurnManager.playerUnit);
        entities.Remove(thisUnit);

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
            Debug.Log("playing " + possiblePlays[0].card.thisCard.displayName + " with " + possiblePlays[0].favor);
            StartCoroutine(AIPlayCard(possiblePlays[0].card, possiblePlays[0].target));
        }
        else
        {
            myHand.DiscardAll();
        }
    }

    IEnumerator AIPlayCard(ICardDisplay cardReference, BattleTileController targetTile)
    {
        ShowAITargeting(cardReference.thisCard.targetRules, transform.position);
        yield return new WaitForSeconds(playDelay);
        ShowAITargeting(cardReference.thisCard.aoePoint, targetTile.transform.position, true);
        if(cardReference.thisCard.aoeSelf != null) ShowAITargeting(cardReference.thisCard.aoeSelf, transform.position, true);
        yield return new WaitForSeconds(playDelay);

        //clear the range display and take the action
        EventManager.clearActivation.Invoke();
        StartCoroutine(myHand.DiscardCard(cardReference, true));
        StartCoroutine(cardReference.thisCard.PlaySequence(thisUnit,targetTile));
    }

    void ShowAITargeting(bool[,] targetRule, Vector3 source, bool aoeMode = false)
    {
        List<GameObject> displayCells = CellTargeting.ConvertMapRuleToTiles(targetRule, source);
        for (int i = 0; i < displayCells.Count; i++)
        {
            BattleTileController cellController = displayCells[i].GetComponent<BattleTileController>();
            if(!aoeMode)
                cellController.HighlightCell();
            else
                cellController.HighlightCellAOE();
        }
    }

    private float CalculateFavor(BattleTileController moveTile, CardPlus card)
    {
        float favor = 0f;
        foreach (CardEffectPlus effect in card.effects)
        {
            Vector2Int moveVect = MapTools.VectorToMap(moveTile.transform.position);
            if (effect.effectClass == CardClass.MOVE)
            {
                favor += EntityScan(moveVect, personality.interestHostile, personality.interestFriendly, personality.proximityHostile, personality.proximityFriendly);
            }
            else if (effect.effectClass == CardClass.SUMMON)
            {
                favor += EntityScan(moveVect, personality.interestHostile, personality.interestFriendly, personality.summonProximityHostile, personality.summonProximityFriendly);
            }
            else
            {
                List<BattleUnit> targetables = CellTargeting.AreaTargets(moveTile.gameObject, gameObject.tag, effect.effectClass, effect.aoe);
                if (effect.effectClass == CardClass.ATTACK)
                    favor += targetables.Count * personality.interestAttack;
                else if (effect.effectClass == CardClass.BUFF)
                    favor += targetables.Count * personality.interestBuff;
            }
        }
        //Debug.Log($"{gameObject.name}: Favor for targeting {moveTile.transform.position} with {card.name} is {favor}.");
        return favor;
    }

    private bool WeAreFriends(GameObject me, GameObject you)
    {
        if(me.CompareTag("Ally"))
        {
            if(you.CompareTag("Ally") || you.CompareTag("Player"))
                return true;
            else
                return false;
        }
        else if(me.CompareTag("Enemy"))
        {
            if(you.CompareTag("Ally") || you.CompareTag("Player"))
                return false;
            else
                return true;
        }
        else
        {
            Debug.LogError("Unrecognized tag checked.");
            return false;
        }
    }

    readonly int maxMoveScore = 5;
    private float DistanceProcessing(float desiredProximity, float pathDistance, float linearDistance)
    {
        float difference = Mathf.Abs(desiredProximity - pathDistance);
        difference = maxMoveScore - difference;
        difference /= linearDistance;
        return difference;
    }

    private float EntityScan(Vector2Int moveVect, float interestHostile, float interestFriendly, float proximityHostile, float proximityFriendly)
    {
        float output = 0;
        for (int i = 0; i < entities.Count; i++)
        {
            GameObject entity = entities[i].gameObject;
            float pathDistance = pathfinder.GetPathLength(moveVect, MapTools.VectorToMap(entity.transform.position));
            float linearDistance = Vector3.Distance(transform.position, entity.transform.position);
            if (!WeAreFriends(gameObject, entity))
            {
                output += interestHostile * DistanceProcessing(proximityHostile, pathDistance, linearDistance);
            }
            else if (WeAreFriends(gameObject, entity))
            {
                output += interestFriendly * DistanceProcessing(proximityFriendly, pathDistance, linearDistance);
            }
        }
        return output;
    }
}

