using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectInject", menuName = "ScriptableObjects/CardEffects/Inject")]
public class EffectInject : CardEffectPlus
{
    enum InjectLocation { DECK, DISCARD, HAND }
    [SerializeField] InjectLocation location;
    readonly Dictionary<InjectLocation, string> locationNames = new()
    {
        {InjectLocation.DECK, "deck" },
        {InjectLocation.DISCARD, "discard"},
        {InjectLocation.HAND, "hand"},
    };
    [SerializeField] CardPlus toInject;
    private void Reset()
    {
        effectClass = CardClass.BUFF;
    }

    public override string GenerateDescription(IPlayerStats player)
    {
        string word;
        if (scalingMultiplier > 1) word = "copies"; else word = "copy";
        return $"adds {scalingMultiplier} {word} of <color=#FFA500>{toInject.displayName}</color> to the target's {locationNames[location]}.";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Inject(scalingMultiplier, target, toInject);
        yield break;
    }

    void Inject(float scale, BattleUnit target, CardPlus toInject)
    {
        int modifier = Mathf.RoundToInt(scale);
        for(int i = 0; i < modifier; i++)
        {
            if(location == InjectLocation.DECK)
            {
                target.myHand.DrawCard(toInject);
            }
            else if(location == InjectLocation.DISCARD) 
        }
    }
}
