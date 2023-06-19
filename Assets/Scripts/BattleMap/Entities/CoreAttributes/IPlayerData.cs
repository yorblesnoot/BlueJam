using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerData
{
    public int damageScaling { get; set; }
    public int healScaling { get; set; }
    public int barrierScaling { get; set; }
    public int maxHealth { get; set; }
    public int currentHealth { get; set; }
}
