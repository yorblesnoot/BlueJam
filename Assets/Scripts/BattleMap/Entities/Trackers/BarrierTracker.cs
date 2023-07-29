using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierTracker : MonoBehaviour
{
    public BattleUnit unitActions;

    [HideInInspector] public List<int> deflectInstances;
    [HideInInspector] public List<int> deflectDurations;

    readonly int deflectLength = 2;

    public void AddDeflect(int amount)
    {
        if (gameObject.CompareTag("Player"))
        {
            Tutorial.Initiate(TutorialFor.BATTLEBARRIER, TutorialFor.BATTLEACTIONS);
            Tutorial.EnterStage(TutorialFor.BATTLEBARRIER, 1, "I've gained a barrier, which absorbs damage before it can reduce my health! Deflect is blue, and only lasts 2 actions. Shield is gray, and lasts til battle's end.");
        }
        deflectInstances.Add(amount);
        deflectDurations.Add(deflectLength);
        unitActions.deflectHealth = GetTotalDeflect();
        unitActions.myUI.UpdateDeflect(-amount);
    }

    public void AddShield(int amount)
    {
        unitActions.shieldHealth += amount;
        unitActions.myUI.UpdateShield(-amount);
    }

    public void DeflectLapse()
    {
        for(int i = 0; i < deflectInstances.Count; i++)
        {
            deflectDurations[i]--;
            if (deflectDurations[i] > 0) continue;
            unitActions.myUI.UpdateDeflect(deflectInstances[i]);
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
        unitActions.myUI.UpdateDeflect(incomingDamage);
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

    public int ReceiveShieldDamage(int incomingDamage)
    {
        unitActions.shieldHealth -= incomingDamage;
        unitActions.myUI.UpdateShield(incomingDamage);
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
