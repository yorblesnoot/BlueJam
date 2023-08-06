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
        ICardDisplay toDiscard;
        List<ICardDisplay> hand = target.myHand.handCards;
        switch (discardType)
        {
            case DiscardType.leftmost:
                toDiscard = hand[0];
                break;
            case DiscardType.rightmost:
                toDiscard = hand.Last();
                break;
            case DiscardType.random:
                toDiscard = hand[Random.Range(0, hand.Count)];
                break;
            default: toDiscard = null; break;
        }

        target.StartCoroutine(target.myHand.DiscardCard(toDiscard, false));
        return toDiscard;
    }
}
