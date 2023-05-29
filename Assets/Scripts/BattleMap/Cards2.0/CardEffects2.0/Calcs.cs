using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calcs
{
    public static int PercentMaxHealth(GameObject target, float percent)
    {
        return Mathf.RoundToInt(target.GetComponent<UnitActions>().maxHealth * percent);
    }
}
