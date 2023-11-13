using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectRepeat", menuName = "ScriptableObjects/CardEffects/Repeat")]
public class EffectRepeat : EffectInject
{
    [SerializeField] bool invertCopy;
    public override string GetEffectDescription(Unit player)
    {
        return $"copy {(invertCopy ? "user" : "target")}'s last played card into {(invertCopy ? "target" : "user")}'s {location.ToString().ToLower()}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) RepeatCard(scalingMultiplier, actor, target);
        yield break;
    }

    void RepeatCard(float number, BattleUnit actor, BattleUnit target)
    {
        if (invertCopy) (actor, target) = (target, actor);
        CardPlus copied = target.lastPlayed;
        Inject(number, target, actor, copied);
    }
}
