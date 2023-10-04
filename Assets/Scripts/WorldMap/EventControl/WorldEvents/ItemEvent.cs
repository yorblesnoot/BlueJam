using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEvent : WorldEvent
{
    public override void Activate(WorldEventHandler eventHandler)
    {
        SoundManager.PlaySound(SoundType.GOTCHEST);
        EventManager.awardItem.Invoke();
        base.Activate(eventHandler);

        EventManager.updateItemUI.AddListener(() => ConfirmItemPicked(eventHandler));
    }

    void ConfirmItemPicked(WorldEventHandler eventHandler)
    {
        eventHandler.eventComplete = true;
        EventManager.updateItemUI.RemoveListener(() => ConfirmItemPicked(eventHandler));
    }

}
