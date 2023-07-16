using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Data;

public class UnitAI : MonoBehaviour
{
    List<CardPlus> cardReferences;

    [SerializeField] AIProfile personality;

    private List<BattleUnit> entities;

    [SerializeField] HandPlus myHand;
    [SerializeField] BattleUnit thisUnit;

    readonly float playDelay = .4f;

    //get information from cards: legal targets, card class, target requirements, object reference

    public void AITakeTurn()
    {
        cardReferences = myHand.currentHand;
        //1. put all possible moves with their respective cards in one place
        //checklegal on every targetRules
        entities = new();
        entities.AddRange(TurnManager.turnTakers);
        entities = entities.Where(t => !(t.isSummoned && t.CompareTag("Ally"))).ToList();
        entities.Add(TurnManager.playerUnit);

        List<BattleTileController> optionTile = new();
        List<float> optionFavor = new();
        List<CardPlus> optionReference = new();
        for(int rule = 0; rule < cardReferences.Count; rule++)
        {
            //use battlemap to find legal cells for every card in hand
            List<GameObject> legalTiles = CellTargeting.ConvertMapRuleToTiles(cardReferences[rule].targetRules, transform.position);

            if (cardReferences[rule].pathCheckForTargets == true) legalTiles = legalTiles.EliminateUnpathable(gameObject);

            int ruleLength = legalTiles.Count;
            for(int x = 0; x < ruleLength; x++)
            {
                BattleTileController addTile = legalTiles[x].GetComponent<BattleTileController>();
                if (CellTargeting.ValidPlay(addTile, thisUnit, cardReferences[rule]))
                { 
                    optionTile.Add(addTile);
                    float inFavor = CalculateFavor(addTile, cardReferences[rule]);
                    optionFavor.Add(inFavor);
                    optionReference.Add(cardReferences[rule]);
                }
            }
        }
        //highest Favor means choose that option
        int numberofOptions = optionFavor.Count;
        if(numberofOptions > 0)
        {
            float highestFavor = optionFavor.Max(); 
            int activateIndex = optionFavor.IndexOf(highestFavor);
            StartCoroutine(AIPlayCard(optionReference[activateIndex], optionTile[activateIndex]));
        }
        else if(numberofOptions == 0)
        {
            //if there are no legal options, mulligan and reset the card registry
            cardReferences = new();
            myHand.DiscardAll();
        }
    }

    IEnumerator AIPlayCard(CardPlus cardReference, BattleTileController targetTile)
    {
        ShowAITargeting(cardReference.targetRules, transform.position);
        yield return new WaitForSeconds(playDelay);
        ShowAITargeting(cardReference.aoePoint, targetTile.transform.position, true);
        if(cardReference.aoeSelf != null) ShowAITargeting(cardReference.aoeSelf, transform.position, true);
        yield return new WaitForSeconds(playDelay);

        //clear the range display and take the action
        EventManager.clearActivation.Invoke();
        myHand.Discard(cardReference, true);
        StartCoroutine(cardReference.PlaySequence(thisUnit,targetTile));
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
            Vector3 moveVect = moveTile.transform.position;
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
        //Debug.Log($"{gameObject.name}: Favor for targeting {moveVect} with card class {cardClass} is {favor}.");
        return favor;
    }

    private bool AreWeFriends(GameObject me, GameObject you)
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

    private float DistanceProcessing(float input)
    {
        input = Mathf.Abs(input);
        float a = 1.6f;
        float b = 3f;
        return Mathf.Pow(a, -(input - b));
    }

    private float EntityScan(Vector3 moveVect, float interestHostile, float interestFriendly, float proximityHostile, float proximityFriendly)
    {
        float output = 0;
        for (int i = 0; i < entities.Count; i++)
        {
            GameObject entity = entities[i].gameObject;
            if (AreWeFriends(this.gameObject, entity) == false)
            {
                float distance = Vector3.Distance(entity.transform.position, moveVect);
                output += interestHostile * DistanceProcessing(proximityHostile - distance);
            }
            else if (AreWeFriends(this.gameObject, entity) == true)
            {
                float distance = Vector3.Distance(entity.transform.position, moveVect);
                output += interestFriendly * DistanceProcessing(proximityFriendly - distance);
            }
        }
        return output;
    }
}

