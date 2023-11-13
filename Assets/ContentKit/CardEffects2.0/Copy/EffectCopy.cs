using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCopy", menuName = "ScriptableObjects/CardEffects/Copy")]
public class EffectCopy : EffectInject
{
    [SerializeField] bool invertCopy;
    public override string GetEffectDescription(Unit player)
    {
        return $"copy a random card from {(invertCopy ? "user" : "target")}'s hand into {(invertCopy ? "target" : "user")}'s {location.ToString().ToLower()}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) CopyCard(scalingMultiplier, actor, target);
        yield break;
    }

    void CopyCard(float number, BattleUnit actor, BattleUnit target)
    {
        if (invertCopy) (actor, target) = (target, actor);
        List<ICardDisplay> targetCards = target.myHand.handCards;
        CardPlus copied = targetCards[Random.Range(0, targetCards.Count)].thisCard;
        Inject(number, target, actor, copied);
    }
}
