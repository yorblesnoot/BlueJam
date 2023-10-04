using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalEvent : WorldEvent
{
    public override void Activate(WorldEventHandler eventHandler)
    {
        SoundManager.PlaySound(SoundType.GOTBOMB);
        runData.RemoveStock++;
        base.Activate(eventHandler);
        eventHandler.eventComplete = true;
    }
}
