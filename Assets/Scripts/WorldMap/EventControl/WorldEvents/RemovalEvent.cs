using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalEvent : WorldEvent
{
    public override void Activate()
    {
        runData.RemoveStock++;
        base.Activate();
    }
}
