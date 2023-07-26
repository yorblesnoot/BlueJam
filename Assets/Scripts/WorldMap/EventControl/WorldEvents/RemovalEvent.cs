using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalEvent : WorldEvent
{
    public override void Activate()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        runData.RemoveStock++;
        base.Activate();
    }
}
