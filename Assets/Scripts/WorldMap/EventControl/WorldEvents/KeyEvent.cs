using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEvent : WorldEvent
{
    public override void Activate()
    {
        runData.KeyStock++;
        base.Activate();
    }
}
