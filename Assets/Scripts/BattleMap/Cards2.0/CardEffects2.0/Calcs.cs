using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calcs
{
    public static int PercentMaxHealth(GameObject targetCell, float percent)
    {
        GameObject target = targetCell.GetComponent<BattleTileController>().unitContents;
        return Mathf.RoundToInt(target.GetComponent<UnitActions>().maxHealth * percent);
    }
}
