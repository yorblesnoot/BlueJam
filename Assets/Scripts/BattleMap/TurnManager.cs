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

    public static List<NonplayerUnit> turnTakers;

    public static PlayerUnit playerUnit;

    public static UnityEvent unitsReport = new();
    public static UnityEvent deathPhase = new();

    public static TurnChange turnChange = new();

    public static UnityEvent updateBeatCounts = new();
    public static UnityEvent initialPositionReport = new();

    public static readonly int beatThreshold = 2;
    readonly static float turnDelay = .5f;

    private void Awake()
    {
        //singleton pattern
        if (Main != null && Main != this) 
            Destroy(this); 
        else 
            Main = this; 

        turnTakers = new();
        playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    public static void InitializeTurns()
    {
        //tell every unit on the map to report their turn
        unitsReport?.Invoke();
        updateBeatCounts?.Invoke();
        initialPositionReport?.Invoke();

        Main.StartCoroutine(WaitForTurn());
    }

    public static void ReportTurn(NonplayerUnit actor)
    { turnTakers.Add(actor); }
    public static void UnreportTurn(NonplayerUnit actor)
    { turnTakers.Remove(actor); }

    static bool PlayerHasWon()
    {
        int enemyCount = 0;
        foreach (NonplayerUnit turnTaker in turnTakers)
        {
            if (turnTaker.gameObject.CompareTag("Enemy"))
                enemyCount++;
        }
        if (enemyCount <= 0)
        {
            BattleEnder battleEnder = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleEnder>();
            battleEnder.StartCoroutine(battleEnder.VictorySequence());
            return true;
        }
        return false;
    }

    public static void ShowPossibleTurnTakers(int beatCost)
    {
        foreach(NonplayerUnit turnTaker in turnTakers.OfType<NonplayerUnit>())
        {
            if (turnTaker.currentBeats + (beatCost * turnTaker.turnSpeed) >= beatThreshold)
            {
                turnTaker.ShowTurnPossibility();
            }
            NonplayerUI npUI = (NonplayerUI)turnTaker.myUI;
            npUI.ShowBeatGhost(beatCost);
        }
    }

    public static void SpendBeats(BattleUnit owner, int beats)
    {
        deathPhase?.Invoke();
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;
        if (owner.gameObject.CompareTag("Player"))
        {
            //distribute beats to all units based on their individual speeds when the player acts
            for(int entry = 0; entry < turnTakers.Count; entry++)
            {
                float beatChange = turnTakers[entry].turnSpeed * beats / playerUnit.turnSpeed;
                turnTakers[entry].currentBeats += beatChange;
            }
        }
        else owner.currentBeats -= beats;
        updateBeatCounts?.Invoke();
        Main.StartCoroutine(WaitForTurn());
    }

    private static IEnumerator WaitForTurn()
    {
        yield return new WaitForSeconds(turnDelay);
        AssignTurn();
    }

    public static void AssignTurn()
    {
        if (PlayerHasWon() || playerUnit.isDead) return;
        turnTakers = turnTakers.OrderByDescending(x => x.currentBeats).ToList();
        if (turnTakers.Count == 0 || turnTakers[0].currentBeats < beatThreshold)
        {
            playerUnit.StartCoroutine(playerUnit.turnIndicator.ShowTurnExclamation());
            PlayerUnit.playerState = PlayerBattleState.IDLE;
            playerUnit.TakeTurn();
            return;
        }
        turnTakers[0].TakeTurn();
    }

}
