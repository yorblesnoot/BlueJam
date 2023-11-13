using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectResource", menuName = "ScriptableObjects/CardEffects/Resource")]
public class EffectResource : CardEffectPlus
{
    [SerializeField] CardEffectPlus resourceEffect;
    private void Reset()
    {
        scalingMultiplier = 1;
    }
    public override void Initialize()
    {
        resourceEffect.Initialize();
        aoe = resourceEffect.aoe;
    }

    public override string GetEffectDescription(Unit player)
    {
        resourceEffect.Initialize();
        return $"{resourceEffect.GetEffectDescription(player)} for" +
            $" {(scalingMultiplier == 1 ? "each time" : $"every {scalingMultiplier} times")}" +
            $" you've played a card of the same name this combat ({player.resourceTracker.GetResource(name)})";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        actor.resourceTracker.AddResource(name);
        int resource = actor.resourceTracker.GetResource(name);
        for (int i = 0; i < resource; i++)
        {
            resourceEffect.Execute(actor, targetCell);
        }
        yield return null;
    }
}
