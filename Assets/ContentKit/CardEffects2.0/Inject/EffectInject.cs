using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectInject", menuName = "ScriptableObjects/CardEffects/Inject")]
public class EffectInject : CardEffectPlus
{

    public enum InjectLocation { DECK, DISCARD, HAND }
    [SerializeField] InjectLocation location;
    readonly Dictionary<InjectLocation, string> locationNames = new()
    {
        {InjectLocation.DECK, "deck" },
        {InjectLocation.DISCARD, "discard"},
        {InjectLocation.HAND, "hand"},
    };
    [SerializeField] CardPlus toInject;
    [SerializeField] bool forceConsume;
    private void Reset()
    {
        effectSound = SoundTypeEffect.INJECT;
        effectClass = CardClass.BUFF;
        forceConsume = true;
    }

    public override string GetEffectDescription(Unit player)
    {
        string copyWord;
        string consumedWord = "";
        if (forceConsume == true) consumedWord = "temporary";
        if (scalingMultiplier > 1) copyWord = "copies"; else copyWord = "copy";
        return $"conjure {scalingMultiplier} {consumedWord} {copyWord} of <color=#FFA500>{toInject.displayName}</color> to target's {locationNames[location]}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Inject(scalingMultiplier, actor, target, toInject);
        yield break;
    }

    void Inject(float scale, BattleUnit actor, BattleUnit target, CardPlus toInject)
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
