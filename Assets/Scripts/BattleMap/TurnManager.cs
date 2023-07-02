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

    public static List<BattleUnit> turnTakers;
    public static List<float> turnRatios;
    public static List<float> beatStock;

    public static BattleUnit activeTurn;
    public static PlayerUnit playerUnit;

    public static UnityEvent unitsReport = new();

    public static UnityEvent deathPhase = new();
    public static UnityEvent drawThenBuffPhase = new();

    public static TurnChange turnChange = new();

    public static UnityEvent updateBeatCounts = new();
    public static UnityEvent initialPositionReport = new();

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
        turnTakers = new();
        turnRatios = new();
        beatStock = new();
        EventManager.initalizeBattlemap.AddListener(InitializeTurns);
        playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    private void InitializeTurns()
    {
        //tell every unit on the map to report their turn
        unitsReport?.Invoke();
        activeTurn = playerUnit;
        activeTurn.myTurn = true;

        updateBeatCounts?.Invoke();
        initialPositionReport?.Invoke();

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
            if (turnTaker.currentBeats + beatCost * turnTaker.turnSpeed >= beatThreshold)
            {
                turnTaker.ShowTurnPossibility();
            }
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
                    float beatChange = turnRatios[entry] * beats;
                    turnTakers[entry].currentBeats += beatChange;
                    beatStock[entry] += beatChange;
                }
            }
        }
        else
        {
            //if the spender is an NPC, deduct from their beat count
            int spenderIndex = turnTakers.IndexOf(owner);
            beatStock[spenderIndex] -= beats;
            owner.currentBeats -= beats;
        }
        updateBeatCounts?.Invoke();
        EndTurn();
        deathPhase?.Invoke();
        Main.StartCoroutine(WaitForTurn());
    }

    private static IEnumerator WaitForTurn()
    {
        int turnDelay = 1;
        yield return new WaitForSeconds(turnDelay);
        AssignTurn();
    }

    private static void AssignTurn()
    {
        activeTurn = NextTurn();
        activeTurn.myTurn = true;
        drawThenBuffPhase?.Invoke();
        deathPhase?.Invoke();
        if (activeTurn.gameObject == null) AssignTurn();
        else turnChange?.Invoke(activeTurn.gameObject);
    }
    
    private static BattleUnit NextTurn()
    {
        float highestBeats = beatStock.Max();
        if (highestBeats >= beatThreshold)
        {
            //enemy turn
            int activeIndex = beatStock.IndexOf(highestBeats);
            return turnTakers[activeIndex];
            //Debug.Log($"assigning {activeTurn.name} with {highestBeats}");
        }
        else
        {
            return playerUnit;
        }
    }

    public static void EndTurn()
    {
        activeTurn = null;
        foreach (BattleUnit v in turnTakers)
        {
            v.myTurn = false;
        }
    }
}
