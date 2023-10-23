using System.Collections;
using UnityEngine;

public class NonplayerUnit : BattleUnit
{
    [SerializeField] GameObject turnShow;
    [SerializeField] UnitAI unitAI;
    [SerializeField] InfoTagControl tagControl;
    public override void Initialize()
    {
        base.Initialize();
        ScaleWithDifficulty(runData.ThreatLevel);

        currentHealth = Mathf.RoundToInt(loadedStats[StatType.MAXHEALTH]);
        myUI.InitializeHealth();
        TurnManager.unitsReport.AddListener(RegisterTurn);
        EventManager.hideTurnDisplay.AddListener(HideTurnPossibility);
        EventManager.clearActivation.AddListener(HideTurnPossibility);
    }
    public void ScaleWithDifficulty(int difficultyFactor)
    {
        loadedStats[StatType.MAXHEALTH] *= 1 + difficultyFactor * Settings.Balance.HealthPerThreat;
        loadedStats[StatType.DAMAGE] *= 1 + difficultyFactor * Settings.Balance.StatPerThreat;
        loadedStats[StatType.BARRIER] *= 1 + difficultyFactor * Settings.Balance.StatPerThreat;
        loadedStats[StatType.HEAL] *= 1 + difficultyFactor * Settings.Balance.StatPerThreat;
        loadedStats[StatType.SPEED] *= 1 + difficultyFactor * Settings.Balance.SpeedPerThreat;
    }

    public override void TakeTurn()
    {
        base.TakeTurn();
        unitAI.AITakeTurn();
    }

    public void RegisterTurn()
    {
        TurnManager.ReportTurn(this);
    }

    public override void SpendBeats(int beats)
    {
        TurnManager.NPCSpendBeats(this, beats);
    }

    public void ShowTurnPossibility()
    {
        turnShow.SetActive(true);
    }

    public void HideTurnPossibility()
    {
        turnShow.SetActive(false);
    }

    public override void ModifyHealth(int reduction)
    {
        base.ModifyHealth(reduction);
        if (reduction <= 0) return;
        Tutorial.Initiate(TutorialFor.BATTLEDODAMAGE, TutorialFor.BATTLEACTIONS);
        Tutorial.EnterStage(TutorialFor.BATTLEDODAMAGE, 1, "Good hit! Reduce the health of all non-summoned enemies to 0 to win the battle!");
    }

    public override void Die()
    {
        base.Die();
        if (gameObject.CompareTag("Enemy") && isSummoned != true)
        {
            //when an enemy dies, add its deck to the player's inventory for later use
            BattleEnder.deckDrops.Add(GetComponent<HandPlus>().deckRecord);
        }
        TurnManager.UnreportTurn(this);
        UnreportCell();
        TurnManager.deathPhase.RemoveListener(CheckForDeath);
        isDead = true;
        StartCoroutine(SlowDeath());
    }

    static readonly float deathPause = .5f;
    IEnumerator SlowDeath()
    {
        yield return new WaitForSeconds(deathPause);
        VFXMachine.PlayAtLocation("Explosion", transform.position);
        gameObject.SetActive(false);
    }

    public override void ShowInfoTag()
    {
        transform.SetAsLastSibling();
        tagControl.ShowTag(this, unitStats);
    }
}