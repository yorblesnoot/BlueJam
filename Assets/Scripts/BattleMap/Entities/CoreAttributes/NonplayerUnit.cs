using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonplayerUnit : BattleUnit
{
    [SerializeField]GameObject turnShow;
    public override void Initialize()
    {
        base.Initialize();
        //correct difficulty scaling source~~~~~~~~~~~~~~~~
        ScaleDifficulty(runData.runDifficulty);

        currentHealth = maxHealth;
        myUI.InitializeHealth();
        EventManager.hideTurnDisplay.AddListener(HideTurnPossibility);
        EventManager.clearActivation.AddListener(HideTurnPossibility);
    }
    public void ScaleDifficulty(int scaleFactor)
    {
        maxHealth *= 1 + Mathf.RoundToInt(scaleFactor * .05f);
        //nonplayerUnit.handSize = 0;
        damageScaling *= 1 + Mathf.RoundToInt(scaleFactor * .1f);
        barrierScaling *= 1 + Mathf.RoundToInt(scaleFactor * .1f);
        healScaling *= 1 + Mathf.RoundToInt(scaleFactor * .1f);
        turnSpeed *= 1 + Mathf.RoundToInt(scaleFactor * .01f);
    }


    public void ShowTurnPossibility()
    {
        turnShow.SetActive(true);
    }

    public void HideTurnPossibility()
    {
        turnShow.SetActive(false);
    }
}