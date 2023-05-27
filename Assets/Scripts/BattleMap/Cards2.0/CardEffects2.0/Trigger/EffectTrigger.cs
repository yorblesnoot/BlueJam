using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectTrigger", menuName = "ScriptableObjects/CardEffects/Trigger")]
public class EffectTrigger : CardEffectPlus
{
    public enum TriggerIdentity
    {
        USER,
        RECEIVER
    }
    public bool purgeable;
    public int duration;

    public TriggerIdentity triggerIdentityCondition;
    public CardEffectPlus triggeringEffect;
    [Range(1, 10)]public int triggersRequiredForActivation;

    public TriggerIdentity effectRecipient;
    public CardEffectPlus triggeredEffect;

    public override string GenerateDescription()
    {
        //generate a description somehow..?
        return "";
    }

    public override void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        List<GameObject> toBuff = ZoneTargeter.AreaTargets(targetCell, actor.tag, effectClass, aoe);
        for (int i = 0; i < toBuff.Count; i++)
        {
            toBuff[i].GetComponent<TriggerTracker>().RegisterTrigger(this);
        }
    }
}
