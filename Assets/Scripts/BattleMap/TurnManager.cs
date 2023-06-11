using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class TurnChange : UnityEvent<GameObject> {}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Main { get; private set; }

    public static List<BattleUnit> turnTakers = new();
    public static List<float> turnRatios = new();
    public static List<float> beatStock = new();

    public static BattleUnit activeTurn;
    public static BattleUnit playerUnit;

    public static UnityEvent unitsReport = new();

    public static UnityEvent deathPhase = new();
    public static UnityEvent drawPhase = new();

    public static TurnChange turnChange = new();

    public static UnityEvent startUpdate = new();
    public static UnityEvent finishUpdate = new();

    public static int beatThreshold = 2;

    private void Awake()
    {
        //singleton pattern
        if (Main != null && Main != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Main = this; 
        }
        EventManager.initalizeBattlemap.AddListener(InitalizeTurns);
        playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<BattleUnit>();
    }

    private void InitalizeTurns()
    {
        //tell every unit on the map to report their turn
        unitsReport?.Invoke();
        activeTurn = playerUnit;
        activeTurn.GetComponent<BattleUnit>().myTurn = true;
        startUpdate?.Invoke();
        finishUpdate?.Invoke();
        StartCoroutine(WaitForTurn());
    }

    public static void ReportTurn(BattleUnit actor)
    {
        //add the reporting unit to the turn list
        turnTakers.Add(actor);
        turnRatios.Add(actor.turnSpeed);
        beatStock.Add(actor.currentBeats);
    }
    public static void UnreportTurn(BattleUnit actor)
    {
        // remove the unit from the turn lists
        int removalIndex = turnTakers.IndexOf(actor);
        Debug.Log(removalIndex);
        turnTakers.RemoveAt(removalIndex);
        turnRatios.RemoveAt(removalIndex);
        beatStock.RemoveAt(removalIndex);

        //count enemies to see if the fight is over
        int enemyCount = 0;
        foreach(BattleUnit turnTaker in turnTakers)
        {
            if (turnTaker.gameObject.tag == "Enemy")
            {
                enemyCount++;
            }
        }

        //if there are no enemies left, end combat
        if (enemyCount == 0)
        {
            EndTurn();
            BattleEnder battleEnder = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleEnder>();
            battleEnder.EndCombat();
        }
    }

    public static void ShowPossibleTurnTakers(int beatCost)
    {
        foreach(NPCBattleUnit turnTaker in turnTakers.OfType<NPCBattleUnit>())
        {
            if(turnTaker.currentBeats + beatCost*turnTaker.turnSpeed >= beatThreshold) turnTaker.ShowTurnPossibility();
        }
    }

    public static void SpendBeats(BattleUnit owner, int beats)
    {
        if(owner.gameObject.tag == "Player")
        {
            //distribute beats to all units based on their individual speeds when the player acts
            for(int entry = 0; entry < turnTakers.Count; entry++)
            {
                if (turnTakers[entry].gameObject.tag != "Player")
                {
                    float beatChange = turnRatios[entry] * (float)beats;
                    turnTakers[entry].currentBeats += beatChange;
                    beatStock[entry] += beatChange;
                }
            }
        }
        else
        {
            //if the spender is an NPC, deduct from their beat count
            int spenderIndex = turnTakers.IndexOf(owner);
            beatStock[spenderIndex] -= (float)beats;
            owner.GetComponent<BattleUnit>().currentBeats -= beats;
        }
        startUpdate?.Invoke();
        EndTurn();
        Main.StartCoroutine(WaitForTurn());
    }

    private static void EndTurn()
    {
        for(int num = 0; num < turnTakers.Count; num++)
        {
            turnTakers[num].GetComponent<BattleUnit>().myTurn = false;
        }
    }

    private static IEnumerator WaitForTurn()
    {
        int turnDelay = 1;
        yield return new WaitForSeconds(turnDelay);
        AssignTurn();
    }

    private static void AssignTurn()
    {
        deathPhase?.Invoke();
        drawPhase?.Invoke();
        float highestBeats = beatStock.Max();
        if(highestBeats >= beatThreshold)
        {
            //enemy turn
            int activeIndex = beatStock.IndexOf(highestBeats);
            activeTurn = turnTakers[activeIndex];

            //Debug.Log($"assigning {activeTurn.name} with {highestBeats}");
        }
        else
        {
            activeTurn = playerUnit;
        }
        activeTurn.GetComponent<BattleUnit>().myTurn = true;
        turnChange?.Invoke(activeTurn.gameObject);
    }
}
