using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectInject", menuName = "ScriptableObjects/CardEffects/Inject")]
public class EffectInject : CardEffectPlus
{

    public enum InjectLocation { DECK, DISCARD, HAND }
    [SerializeField] internal InjectLocation location;
    [SerializeField] CardPlus toInject;
    [SerializeField] internal bool forceConsume;
    private void Reset()
    {
        scalingMultiplier = 1;
        effectSound = SoundTypeEffect.INJECT;
        effectClass = CardClass.BUFF;
        forceConsume = true;
    }

    public override string GetEffectDescription(Unit player)
    {
        return $"conjure {scalingMultiplier} {(forceConsume? "temporary" : "")} {(scalingMultiplier > 1 ? "copies" : "copy")} of <color=#FFA500>{toInject.displayName}</color> to target's {location.ToString().ToLower()}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Inject(scalingMultiplier, actor, target, toInject);
        yield break;
    }

    internal void Inject(float scale, BattleUnit actor, BattleUnit target, CardPlus toInject)
    {
        HandPlus hand = target.myHand;
        int modifier = Mathf.RoundToInt(scale);
        for(int i = 0; i < modifier; i++)
        {
            ICardDisplay card = hand.ConjureCard(toInject, actor.transform.position, location);
            card.forceConsume = forceConsume;
        }
    }
}
