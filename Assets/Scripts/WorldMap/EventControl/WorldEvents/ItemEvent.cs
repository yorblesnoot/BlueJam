using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEvent : WorldEvent
{
    public override void Activate()
    {
        EventManager.awardItem.Invoke();
        base.Activate();
    }
}
