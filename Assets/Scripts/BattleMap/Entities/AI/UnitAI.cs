using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using System.Data;

public class UnitAI : MonoBehaviour
{
    List<CardPlus> cardReferences;

    public AIProfile personality;

    private List<GameObject> entities;

    public Hand myHand;

    // Start is called before the first frame update
    void Awake()
    {
        TurnManager.turnChange.AddListener(TakeTurn);
    }

    //get information from cards: legal targets, card class, target requirements, object reference

    public void TakeTurn(GameObject active)
    {
        if(gameObject == active)
        {
            cardReferences = myHand.handReferences;
            //1. put all possible moves with their respective cards in one place
            //checklegal on every targetRules
            entities = TurnManager.turnTakers;

            List<GameObject> optionTile = new List<GameObject>();
            List<float> optionFavor = new List<float>();
            List<CardPlus> optionReference = new List<CardPlus>();
            for(int rule = 0; rule < cardReferences.Count; rule++)
            {
                //use battlemap to find legal cells for every card in hand
                List<GameObject> legalTiles = ZoneTargeter.CheckLegal(cardReferences[rule].targetRules, transform.position);
                int ruleLength = legalTiles.Count;
                for(int x = 0; x < ruleLength; x++)
                {
                    if (ZoneTargeter.ValidPlay(legalTiles[x], gameObject.tag, cardReferences[rule].cardClass, cardReferences[rule].aoeRules))
                    { 
                        GameObject addTile = legalTiles[x];
                        optionTile.Add(addTile);
                        float inFavor = CalculateFavor(addTile, cardReferences[rule].cardClass, cardReferences[rule].aoeRules);
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
                cardReferences = new List<CardPlus>();
                gameObject.GetComponent<Hand>().DiscardAll();
            }
        }
    }

    IEnumerator AIPlayCard(CardPlus cardReference, GameObject targetTile)
    {
        //highlight the range for an AI card briefly
        List<GameObject> displayCells = ZoneTargeter.CheckLegal(cardReference.targetRules, transform.position);
        for (int i = 0; i < displayCells.Count; i++)
        {
            BattleTileController cellController = displayCells[i].GetComponent<BattleTileController>();
            cellController.HighlightCell();
        }
        float playDelay = .8f;
        yield return new WaitForSeconds(playDelay);

        //clear the range display and take the action
        EventManager.clearActivation.Invoke();
        StartCoroutine(cardReference.PlaySequence(gameObject,targetTile));
        myHand.Discard(cardReference, true);
    }

    private float CalculateFavor(GameObject moveTile, List<CardClass> cardClass, string[,] cardAOE)
    {
        float favor = 0f;
        Vector3 moveVect = moveTile.transform.position;

        if (cardClass.Contains(CardClass.MOVE))
        {
            favor += EntityScan(moveVect, personality.interestHostile, personality.interestFriendly, personality.proximityHostile, personality.proximityFriendly);
        }
        if(cardClass.Contains(CardClass.SUMMON))
        {
            favor += EntityScan(moveVect, personality.interestHostile, personality.interestFriendly, personality.summonProximityHostile, personality.summonProximityFriendly);
        }
        if(cardClass.Contains(CardClass.ATTACK))
        {
            List <GameObject> attackables = ZoneTargeter.AreaTargets(moveTile, gameObject.tag, CardClass.ATTACK, cardAOE);
            favor += attackables.Count * personality.interestAttack;
        }
        if (cardClass.Contains(CardClass.BUFF))
        {
            List<GameObject> buffables = ZoneTargeter.AreaTargets(moveTile, gameObject.tag, CardClass.BUFF, cardAOE);
            favor += buffables.Count * personality.interestBuff;
        }

        //Debug.Log($"{gameObject.name}: Favor for targeting {moveVect} with card class {cardClass} is {favor}.");
        return favor;
    }

    private bool AreWeFriends(GameObject me, GameObject you)
    {
        if(me.tag == "Ally")
        {
            if(you.tag == "Ally" || you.tag == "Player")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(me.tag == "Enemy")
        {
            if(you.tag == "Ally" || you.tag == "Player")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            Debug.Log("Unrecognized tag checked.");
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
            GameObject entity = entities[i];
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

