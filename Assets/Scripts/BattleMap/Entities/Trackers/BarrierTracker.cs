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
        deflectInstances.Add(amount);
        deflectDurations.Add(deflectLength);
        unitActions.deflectHealth = GetTotalDeflect();
        unitActions.myUI.UpdateHealth();
    }

    public void AddShield(int amount)
    {
        unitActions.shieldHealth += amount;
        unitActions.myUI.UpdateHealth();
    }

    public void DeflectLapse()
    {
        for(int i = 0; i < deflectInstances.Count; i++)
        {
            deflectDurations[i]--;
            if (deflectDurations[i] <= 0) RemoveDeflectInstance(i);
        }
        unitActions.deflectHealth = GetTotalDeflect();
    }
    public void RemoveDeflectInstance(int i)
    {
        deflectInstances.RemoveAt(i);
        deflectDurations.RemoveAt(i);
        unitActions.deflectHealth = GetTotalDeflect();
        Debug.Log(unitActions.deflectHealth);
        unitActions.myUI.UpdateHealth();
    }

    public int ReceiveDeflectDamage(int incomingDamage)
    {
        while(incomingDamage > 0 && deflectInstances.Count > 0)
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
