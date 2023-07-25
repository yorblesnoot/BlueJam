using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitStats
{
    public Dictionary<StatType, float> loadedStats {  get; set; }
    public int currentHealth { get; set; }
}
