using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonplayerUnit : BattleUnit
{
    [SerializeField] GameObject turnShow;
    [SerializeField] UnitAI unitAI;
    public override void Initialize()
    {
        base.Initialize();
        ScaleWithDifficulty(runData.runDifficulty);

        currentHealth = maxHealth;
        myUI.InitializeHealth();
        TurnManager.unitsReport.AddListener(RegisterTurn);
        EventManager.hideTurnDisplay.AddListener(HideTurnPossibility);
        EventManager.clearActivation.AddListener(HideTurnPossibility);
    }
    public void ScaleWithDifficulty(int difficultyFactor)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * (1 + (difficultyFactor * .05f)));
        DamageScaling = Mathf.RoundToInt(DamageScaling * (1 + (difficultyFactor * .1f)));
        barrierScaling = Mathf.RoundToInt(barrierScaling * (1 + (difficultyFactor * .1f)));
        healScaling = Mathf.RoundToInt(healScaling * (1 + (difficultyFactor * .1f)));
        turnSpeed *= 1 + (difficultyFactor * .01f);
    }

    public override void GetAction()
    {
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