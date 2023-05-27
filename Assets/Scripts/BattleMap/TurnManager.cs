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

    public static List<GameObject> turnTakers;
    public static List<float> turnRatios;
    public static List<float> beatStock;

    public static GameObject activeTurn;
    public static GameObject playerUnit;

    public static UnityEvent unitsReport = new UnityEvent();

    public static UnityEvent turnDraw = new UnityEvent();
    public static TurnChange turnChange = new TurnChange();

    public static UnityEvent startUpdate = new UnityEvent();
    public static UnityEvent finishUpdate = new UnityEvent();

    public static int beatThreshold;

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
        turnTakers = new List<GameObject>();
        turnRatios = new List<float>();
        beatStock = new List<float>();
        EventManager.initalizeBattlemap.AddListener(InitalizeTurns);
        playerUnit = GameObject.FindGameObjectWithTag("Player");

        beatThreshold = 2;
    }

    private void InitalizeTurns()
    {
        //tell every unit on the map to report their turn
        unitsReport?.Invoke();
        activeTurn = GameObject.FindGameObjectWithTag("Player");
        activeTurn.GetComponent<UnitActions>().myTurn = true;
        startUpdate?.Invoke();
        finishUpdate?.Invoke();
        StartCoroutine(WaitForTurn());
    }

    public static void ReportTurn(GameObject actor)
    {
        //add the reporting unit to the turn list
        turnTakers.Add(actor);
        UnitActions unitActions = actor.GetComponent<UnitActions>();
        turnRatios.Add(unitActions.turnSpeed);
        beatStock.Add(unitActions.currentBeats);
    }
    public static void UnreportTurn(GameObject actor)
    {
        // remove the unit from the turn lists
        int removalIndex = turnTakers.IndexOf(actor);
        turnTakers.RemoveAt(removalIndex);
        turnRatios.RemoveAt(removalIndex);
        beatStock.RemoveAt(removalIndex);

        //count enemies to see if the fight is over
        int enemyCount = 0;
        foreach(GameObject turnTaker in turnTakers)
        {
            if (turnTaker.tag == "Enemy")
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

    public static void SpendBeats(GameObject owner, int beats)
    {
        if(owner.tag == "Player")
        {
            //distribute beats to all units based on their individual speeds when the player acts
            for(int entry = 0; entry < turnTakers.Count; entry++)
            {
                if (turnTakers[entry].tag != "Player")
                {
                    float beatChange = turnRatios[entry] * (float)beats;
                    turnTakers[entry].GetComponent<UnitActions>().currentBeats += beatChange;
                    beatStock[entry] += beatChange;
                }
            }
        }
        else
        {
            //if the spender is an NPC, deduct from their beat count
            int spenderIndex = turnTakers.IndexOf(owner);
            beatStock[spenderIndex] -= (float)beats;
            owner.GetComponent<UnitActions>().currentBeats -= beats;
        }
        startUpdate?.Invoke();
        EndTurn();
        turnDraw?.Invoke();
        Main.StartCoroutine(WaitForTurn());
    }

    private static void EndTurn()
    {
        for(int num = 0; num < turnTakers.Count; num++)
        {
            turnTakers[num].GetComponent<UnitActions>().myTurn = false;
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
        beatThreshold = 2;
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
            //player turn
            activeTurn = playerUnit;
        }
        activeTurn.GetComponent<UnitActions>().myTurn = true;
        turnDraw?.Invoke();
        if(activeTurn.GetComponent<UnitActions>().isDead != true) turnChange?.Invoke(activeTurn);
        else AssignTurn();
    }
}
