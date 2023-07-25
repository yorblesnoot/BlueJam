using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calcs
{
    public static int PercentMaxHealth(BattleUnit target, float percent)
    {
        return Mathf.RoundToInt(target.loadedStats[StatType.MAXHEALTH] * percent);
    }
}
