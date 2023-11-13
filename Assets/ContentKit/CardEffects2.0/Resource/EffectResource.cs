using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            $" you've played a card of the same name this combat (<color=red>{player.resourceTracker.GetResource(name)+1}x</color>)";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        actor.resourceTracker.AddResource(name);
        int resource = actor.resourceTracker.GetResource(name);
        for (int i = 0; i < resource; i++)
        {
            yield return actor.StartCoroutine(resourceEffect.Execute(actor, targetCell));
        }
        if (actor.Allegiance == AllegianceType.PLAYER) ((PlayerHandPlus)actor.myHand).UpdateHand();
    }
}
