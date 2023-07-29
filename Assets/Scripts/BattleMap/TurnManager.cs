using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class BeatUpdate : UnityEvent<float> { }
public class TurnManager : MonoBehaviour
{
    public static TurnManager Main { get; private set; }

    public static List<NonplayerUnit> turnTakers;

    public static PlayerUnit playerUnit;

    public static UnityEvent unitsReport = new();
    public static UnityEvent deathPhase = new();

    public static BeatUpdate updateBeatCounts = new();
    public static UnityEvent initialPositionReport = new();

    public static readonly int beatThreshold = 2;
    readonly static float turnDelay = .2f;

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

    public static void InitializePositions()
    {
        //tell every unit on the map to report their turn
        unitsReport?.Invoke();
        initialPositionReport?.Invoke();
    }

    public static void InitializeTurns()
    {
        InitializeDecks(playerUnit);
        foreach (var unit in turnTakers)
        {
            InitializeDecks(unit);
        }
        Main.StartCoroutine(WaitForTurn());

        static void InitializeDecks(BattleUnit unit)
        {
            unit.myHand.BuildVisualDeck();
            unit.myHand.DrawPhase();
        }
    }

    public static void ReportTurn(NonplayerUnit actor)
    { turnTakers.Add(actor); }
    public static void UnreportTurn(NonplayerUnit actor)
    { turnTakers.Remove(actor); }

    static bool PlayerHasWon()
    {
        if (turnTakers.Where(x => x.gameObject.CompareTag("Enemy") && x.isSummoned != true).Count() == 0)
        {
            BattleEnder battleEnder = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleEnder>();
            battleEnder.StartCoroutine(battleEnder.VictorySequence());
            return true;
        }
        return false;
    }

    public static void ShowPossibleTurnTakers(int beatCost)
    {
        foreach(NonplayerUnit turnTaker in turnTakers)
        {
            float expenditure = GetBeatCost(beatCost, turnTaker, playerUnit);
            if (turnTaker.loadedStats[StatType.BEATS] + expenditure >= beatThreshold)
            {
                turnTaker.ShowTurnPossibility();
            }
            NonplayerUI npUI = (NonplayerUI)turnTaker.myUI;
            npUI.ShowBeatGhost(expenditure);
        }
    }

    public static void NPCSpendBeats(NonplayerUnit owner, int beats)
    {
        owner.myHand.DrawPhase();
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;

        owner.loadedStats[StatType.BEATS] -= beats;
        EntityUI npUI = owner.myUI;
        npUI.UpdateBeats(beats);
        FinalizeTurn(owner);
    }

    public static void PlayerSpendBeats(int beats)
    {
        playerUnit.myHand.DrawPhase();
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;

        Tutorial.CompleteStage(TutorialFor.BATTLEDAMAGE, 1, true);
        //distribute beats to all units based on their individual speeds when the player acts
        foreach (NonplayerUnit turnTaker in turnTakers)
        {
            float beatChange = GetBeatCost(beats, turnTaker, playerUnit);
            turnTaker.loadedStats[StatType.BEATS] += beatChange;
            EntityUI npUI = turnTaker.myUI;
            npUI.UpdateBeats(-beatChange);
        }
        FinalizeTurn(playerUnit);
    }

    static void FinalizeTurn(BattleUnit owner)
    {
        owner.buffTracker.DurationProc();
        deathPhase?.Invoke();
        Main.StartCoroutine(WaitForTurn());
    }

    static float GetBeatCost(int beats, BattleUnit unit, PlayerUnit player)
    {
        return unit.loadedStats[StatType.SPEED] * beats / player.loadedStats[StatType.SPEED]; 
    }

    private static IEnumerator WaitForTurn()
    {
        yield return new WaitForSeconds(turnDelay);
        AssignTurn();
    }

    public static void AssignTurn()
    {
        if (PlayerHasWon() || playerUnit.isDead) return;
        turnTakers = turnTakers.OrderByDescending(x => x.loadedStats[StatType.BEATS]).ToList();
        if (turnTakers.Count == 0 || turnTakers[0].loadedStats[StatType.BEATS] < beatThreshold)
        {
            playerUnit.StartCoroutine(playerUnit.turnIndicator.ShowTurnExclamation());
            PlayerUnit.playerState = PlayerBattleState.IDLE;
            playerUnit.TakeTurn();
            return;
        }
        turnTakers[0].TakeTurn();
    }

}
