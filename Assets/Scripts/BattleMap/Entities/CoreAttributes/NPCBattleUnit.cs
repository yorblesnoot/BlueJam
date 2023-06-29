using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBattleUnit : BattleUnit
{
    [SerializeField]GameObject turnShow;
    public override void Initialize()
    {
        base.Initialize();
        currentHealth = maxHealth;
        myUI.InitializeHealth();
        EventManager.clearActivation.AddListener(HideTurnPossibility);
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