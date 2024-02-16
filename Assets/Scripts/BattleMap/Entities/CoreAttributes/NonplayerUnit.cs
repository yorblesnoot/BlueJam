using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class NonplayerUnit : BattleUnit, ITurnTakingNonplayer
{
    [SerializeField] GameObject turnShow;
    [SerializeField] UnitAI unitAI;
    [SerializeField] InfoTagControl tagControl;

    public float TurnThreshold => TurnManager.beatThreshold;
    public override void Initialize()
    {
        base.Initialize();
        ScaleWithDifficulty(runData.ThreatLevel);

        currentHealth = Mathf.RoundToInt(loadedStats[StatType.MAXHEALTH]);
        myUI.InitializeHealth();
        TurnManager.allUnitsReportTurns.AddListener(() => TurnManager.ReportTurn(this));
        EventManager.hideTurnDisplay.AddListener(() => ShowBeatPreview(0));
        EventManager.clearActivation.AddListener(() => ShowBeatPreview(0));
    }
    public void ScaleWithDifficulty(int difficultyFactor)
    {
        loadedStats[StatType.MAXHEALTH] *= 1 + difficultyFactor * Settings.Balance[BalanceParameter.EnemyHealthScaling];
        loadedStats[StatType.DAMAGE] *= 1 + difficultyFactor * Settings.Balance[BalanceParameter.EnemyStatScaling];
        loadedStats[StatType.BARRIER] *= 1 + difficultyFactor * Settings.Balance[BalanceParameter.EnemyStatScaling];
        loadedStats[StatType.HEAL] *= 1 + difficultyFactor * Settings.Balance[BalanceParameter.EnemyStatScaling];
        loadedStats[StatType.SPEED] *= 1 + difficultyFactor * Settings.Balance[BalanceParameter.EnemySpeedScaling];
    }

    public override void TakeTurn()
    {
        base.TakeTurn();
        NonplayerUI nyUI = (NonplayerUI)myUI;
        nyUI.HideBeatGhost();
        unitAI.AITakeTurn();
    }

    public void ReceiveBeatsFromPlayer(int beats, PlayerUnit player)
    {
        float beatChange = GetBeatChange(beats, player);
        loadedStats[StatType.BEATS] += beatChange;
        myUI.ReduceBeats(-beatChange);
    }

    public float BeatCount => loadedStats[StatType.BEATS];

    public string GetAllegiance()
    {
        return gameObject.tag;
    }

    float GetBeatChange(int beats, PlayerUnit player)
    {
        return loadedStats[StatType.SPEED] * beats / player.loadedStats[StatType.SPEED];
    }

    public override void SpendBeats(int beats)
    {
        turnShow.SetActive(false);
        TurnManager.NPCSpendBeats(this, beats);
    }

    public void ShowBeatPreview(int beats)
    {
        float expenditure = GetBeatChange(beats, TurnManager.playerUnit);
        turnShow.SetActive(loadedStats[StatType.BEATS] + expenditure >= TurnManager.beatThreshold);
        NonplayerUI npUI = (NonplayerUI)myUI;
        npUI.ShowBeatGhost(expenditure);
    }

    public override void ModifyHealth(int reduction)
    {
        base.ModifyHealth(reduction);
        if (reduction <= 0) return;
        Tutorial.Initiate(TutorialFor.BATTLEDODAMAGE, TutorialFor.BATTLEACTIONS);
        Tutorial.EnterStage(TutorialFor.BATTLEDODAMAGE, 1, "Good hit! Reduce the health of all non-summoned enemies to 0 to win!");
    }

    public override void Die()
    {
        base.Die();
        if (gameObject.CompareTag("Enemy") && isSummoned != true)
        {
            //when an enemy dies, add its deck to the player's inventory for later use
            BattleEnder.deckDrops.Add(GetComponent<HandPlus>().deckRecord);
        }
        TurnManager.RemoveFromTurnOrder(this);
        MapTools.ReportDepartureFromMap(this);
        TurnManager.globalDeathCheck.RemoveListener(CheckForDeath);
        isDead = true;
        StartCoroutine(SlowDeath());
    }

    static readonly float deathPause = 1f;
    IEnumerator SlowDeath()
    {
        if(!isSummoned && (Allegiance == AllegianceType.SLIME || Allegiance == AllegianceType.SLIME)) VFXMachine.PlayAtLocation("DeathHaze", transform.position);
        yield return new WaitForSeconds(deathPause);
        gameObject.SetActive(false);
    }

    public override void ShowInfoTag()
    {
        transform.SetAsLastSibling();
        tagControl.ShowTag(this, unitStats);
    }
}