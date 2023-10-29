using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Main { get; private set; }

    public static List<ITurnTaker> turnTakers;

    public static PlayerUnit playerUnit;

    public static UnityEvent unitsReport = new();
    public static UnityEvent deathPhase = new();

    public static UnityEvent initialPositionReport = new();
    public static UnityEvent initializeDecks = new();

    public static readonly int beatThreshold = 2;
    readonly static float turnDelay = .2f;

    private void Awake()
    {
        Main = this; 

        turnTakers = new();
        playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();
    }

    public static void InitializePositions()
    {
        //tell every unit on the map to report their turn
        unitsReport?.Invoke();
        initialPositionReport?.Invoke();
    }

    public static void InitializeTurns()
    {
        initializeDecks?.Invoke();
        Main.StartCoroutine(WaitForTurn());
    }

    public static void ReportTurn(ITurnTaker actor)
    { turnTakers.Add(actor); }
    public static void UnreportTurn(ITurnTaker actor)
    { turnTakers.Remove(actor); }

    static bool PlayerHasWon()
    {
        if (turnTakers.Where(x => x.Allegiance == AllegianceType.ENEMY && x.isSummoned != true).Count() == 0)
        {
            BattleEnder battleEnder = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleEnder>();
            battleEnder.StartCoroutine(battleEnder.VictorySequence());
            return true;
        }
        return false;
    }

    public static void ShowPossibleTurnTakers(int beatCost)
    {
        foreach(var turnTaker in turnTakers)
            turnTaker.ShowBeatPreview(beatCost);
    }

    public static void NPCSpendBeats(NonplayerUnit owner, int beats)
    {
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;

        owner.loadedStats[StatType.BEATS] -= beats;
        EntityUI npUI = owner.myUI;
        npUI.ReduceBeats(beats);
    }

    public static void PlayerSpendBeats(int beats)
    {
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;
        Tutorial.CompleteStage(TutorialFor.BATTLEDAMAGE, 1, true);
        //distribute beats to all units based on their individual speeds when the player acts
        foreach (var turnTaker in turnTakers)
        {
            turnTaker.ReceiveBeatsFromPlayer(beats, playerUnit);
        }
    }

    public static void FinalizeTurn(BattleUnit owner)
    {
        owner.myHand.DrawPhase();
        owner.buffTracker.DurationProc();
        deathPhase?.Invoke();
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
        turnTakers = turnTakers.OrderByDescending(x =>  x.BeatCount - x.TurnThreshold).ToList();
        if (turnTakers.Count == 0 || turnTakers[0].BeatCount < turnTakers[0].TurnThreshold)
        {
            playerUnit.StartCoroutine(playerUnit.turnIndicator.ShowTurnExclamation());
            PlayerUnit.playerState = PlayerBattleState.IDLE;
            playerUnit.TakeTurn();
            return;
        }
        turnTakers[0].TakeTurn();
    }

}
