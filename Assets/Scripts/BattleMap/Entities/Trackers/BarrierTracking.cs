using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierTracker
{
    public BattleUnit unitActions;

    [HideInInspector] public List<int> deflectInstances = new();
    [HideInInspector] public List<int> deflectDurations = new();

    readonly int deflectLength = 2;

    public BarrierTracker(BattleUnit battleUnit)
    {
        unitActions = battleUnit;
        if (unitActions.Allegiance != AllegianceType.PLAYER) deflectLength = 1;
    }

    public void AddDeflect(int amount)
    {
        if (unitActions.Allegiance == AllegianceType.PLAYER)
        {
            Tutorial.Initiate(TutorialFor.BATTLEBARRIER, TutorialFor.BATTLEACTIONS);
            Tutorial.EnterStage(TutorialFor.BATTLEBARRIER, 1, 
                "I've gained a barrier, which absorbs damage before it can reduce my health! <color=blue>Block</color> lasts for two turns, and <color=grey>shield</color> lasts indefinitely. You can see my barriers in the bottom left.");
        }
        deflectInstances.Add(amount);
        deflectDurations.Add(deflectLength);
        unitActions.deflectHealth = GetTotalDeflect();
    }

    public void AddShield(int amount)
    {
        unitActions.shieldHealth += amount;
    }

    public void DeflectLapse()
    {
        for(int i = 0; i < deflectInstances.Count; i++)
        {
            deflectDurations[i]--;
            if (deflectDurations[i] > 0) continue;
            RemoveDeflectInstance(i);
        }
        unitActions.deflectHealth = GetTotalDeflect();
    }
    public void RemoveDeflectInstance(int i)
    {
        deflectInstances.RemoveAt(i);
        deflectDurations.RemoveAt(i);
        unitActions.deflectHealth = GetTotalDeflect();
    }

    public int ReceiveDeflectDamage(int incomingDamage)
    {
        if(incomingDamage <= 0) return 0;
        unitActions.stateFeedback.QueuePopup(Mathf.Clamp(incomingDamage, 0, unitActions.deflectHealth), Color.cyan);
        while (incomingDamage > 0 && deflectInstances.Count > 0)
        {
            deflectInstances[0] -= incomingDamage;
            if (deflectInstances[0] <= 0)
            {
                incomingDamage = Mathf.Abs(deflectInstances[0]);
                RemoveDeflectInstance(0);
            }
            else incomingDamage = 0;
        }
        unitActions.deflectHealth = GetTotalDeflect();
        return incomingDamage;
    }

    Color shieldColor = new(0.05f, 0.05f, 0.05f, 1);
    public int ReceiveShieldDamage(int incomingDamage)
    {
        if(incomingDamage <= 0) return 0; 
        unitActions.stateFeedback.QueuePopup(Mathf.Clamp(incomingDamage, 0, unitActions.shieldHealth), shieldColor);
        unitActions.shieldHealth -= incomingDamage;
        if (unitActions.shieldHealth <= 0)
        {
            incomingDamage = Mathf.Abs(unitActions.shieldHealth);
            unitActions.shieldHealth = 0;
            return incomingDamage;
        }
        else return 0;
    }

    public int GetTotalDeflect()
    {
        int totalDeflect = 0;
        for (int i = 0; i < deflectInstances.Count; i++)
        {
            totalDeflect += deflectInstances[i];
        }
        return totalDeflect;
    }
}
