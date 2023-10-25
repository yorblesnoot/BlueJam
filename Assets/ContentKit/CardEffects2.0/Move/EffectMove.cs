using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectMove", menuName = "ScriptableObjects/CardEffects/Move")]
public class EffectMove : CardEffectPlus
{
    public bool blink;
    private void Reset()
    {
        blink = false;
        effectSound = SoundTypeEffect.WALK;
        effectClass = CardClass.MOVE;
    }
    [Range(0f, 1f)] public float moveDuration;
    public override string GetEffectDescription(Unit player)
    {
        string word = "move";
        if (blink) word = "blink";
        return $"{word} to target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        yield return actor.StartCoroutine(Move(actor, targetCell));
    }

    public IEnumerator Move(BattleUnit actor, BattleTileController targetCell)
    {
        MapTools.ReportPositionChange(actor, targetCell);
        if (blink) yield return actor.StartCoroutine(Blink(actor, targetCell));
        else yield return actor.StartCoroutine(Walk(actor, targetCell));
    }

    public IEnumerator Walk(BattleUnit actor, BattleTileController targetCell)
    {
        yield return new WaitForSeconds(.1f);
        yield return actor.StartCoroutine(actor.gameObject.LerpTo(targetCell.unitPosition, moveDuration));
        actor.StartCoroutine(actor.unitAnimator.EndWalk(moveDuration));
    }

    public IEnumerator Blink(BattleUnit actor, BattleTileController targetCell)
    {
        Vector3 destination = targetCell.unitPosition;
        actor.transform.position = new Vector3(100, 100, 100);
        yield return new WaitForSeconds(moveDuration);
        actor.transform.position = destination;
    }
}
