using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCActions : UnitActions
{
    public override void Initialize()
    {
        base.Initialize();
        currentHealth = maxHealth;
    }
}