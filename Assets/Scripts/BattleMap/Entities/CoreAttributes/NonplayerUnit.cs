using UnityEngine;

public class NonplayerUnit : BattleUnit
{
    [SerializeField] GameObject turnShow;
    [SerializeField] UnitAI unitAI;
    public override void Initialize()
    {
        base.Initialize();
        ScaleWithDifficulty(runData.runDifficulty);

        currentHealth = Mathf.RoundToInt(loadedStats[StatType.MAXHEALTH]);
        myUI.InitializeHealth();
        TurnManager.unitsReport.AddListener(RegisterTurn);
        EventManager.hideTurnDisplay.AddListener(HideTurnPossibility);
        EventManager.clearActivation.AddListener(HideTurnPossibility);
    }
    public void ScaleWithDifficulty(int difficultyFactor)
    {
        loadedStats[StatType.MAXHEALTH] *= 1 + difficultyFactor * Settings.Dev.HealthPerThreat;
        loadedStats[StatType.DAMAGE] *= 1 + difficultyFactor * Settings.Dev.StatPerThreat;
        loadedStats[StatType.BARRIER] *= 1 + difficultyFactor * Settings.Dev.StatPerThreat;
        loadedStats[StatType.HEAL] *= 1 + difficultyFactor * Settings.Dev.StatPerThreat;
        loadedStats[StatType.SPEED] *= 1 + difficultyFactor * Settings.Dev.SpeedPerThreat;
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

    public void ShowTurnPossibility()
    {
        turnShow.SetActive(true);
    }

    public void HideTurnPossibility()
    {
        turnShow.SetActive(false);
    }

    public override void ReduceHealth(int reduction)
    {
        base.ReduceHealth(reduction);
        if (reduction <= 0) return;
        Tutorial.Initiate(TutorialFor.BATTLEDODAMAGE, TutorialFor.BATTLEACTIONS);
        Tutorial.EnterStage(TutorialFor.BATTLEDODAMAGE, 1, "Good hit! Reduce the health of all non-summoned enemies to 0 to win the battle!");
    }

    public override void Die()
    {
        if (gameObject.CompareTag("Enemy") && isSummoned != true)
        {
            //when an enemy dies, add its deck to the player's inventory for later use
            BattleEnder.deckDrops.Add(GetComponent<HandPlus>().deckRecord);
        }
        TurnManager.UnreportTurn(this);
        UnreportCell();
        TurnManager.deathPhase.RemoveListener(CheckForDeath);
        isDead = true;
        VFXMachine.PlayAtLocation("Explosion", transform.position);
        gameObject.SetActive(false);
    }
}