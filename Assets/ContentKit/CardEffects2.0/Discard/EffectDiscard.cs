using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDiscard", menuName = "ScriptableObjects/CardEffects/Discard")]
public class EffectDiscard : CardEffectPlus
{
    enum DiscardType { leftmost, rightmost, random }
    [SerializeField] DiscardType discardType;
    private void Reset()
    {
        effectSound = SoundTypeEffect.INJECT;
        effectClass = CardClass.ATTACK;
    }

    public override string GetEffectDescription(Unit player)
    {
        return $"target discards {discardType} card";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Discard(target);
        yield break;
    }

    public ICardDisplay Discard(BattleUnit target)
    {
        List<ICardDisplay> hand = target.myHand.handCards;
        ICardDisplay toDiscard = discardType switch
        {
            DiscardType.leftmost => hand[0],
            DiscardType.rightmost => hand.Last(),
            DiscardType.random => hand[Random.Range(0, hand.Count)],
            _ => null,
        };
        target.StartCoroutine(target.myHand.DiscardCard(toDiscard, false));
        return toDiscard;
    }
}
