using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectPushPull", menuName = "ScriptableObjects/CardEffects/PushPull")]
public class EffectPushPull : CardEffectPlus
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.PUSH;
        effectClass = CardClass.ATTACK;
        duration = .3f;
    }
    [Range(.1f, 1f)] public float duration;
    public override string GetEffectDescription(Unit player)
    {
        string verb;
        if (scalingMultiplier > 0) verb = "push";
        else verb = "pull";
        return $"{verb} target {Mathf.Abs(Mathf.RoundToInt(scalingMultiplier))} cells";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        int pushDistance = Mathf.RoundToInt(scalingMultiplier);
        targets = targets.OrderBy(x => Vector3.Distance(actor.transform.position, x.transform.position)).ToList();
        if(pushDistance > 0) targets.Reverse();
        foreach (BattleUnit target in targets)
        {
            if (target.immovable) continue;
            yield return target.StartCoroutine(Push(actor, target, pushDistance, duration));
        }
    }

    IEnumerator Push(BattleUnit actor, BattleUnit target, int directedDistance, float duration)
    {
        Vector2 direction;
        direction = target.MapPosition() - actor.MapPosition();
        direction *= directedDistance;
        direction.Normalize();
        int distance = Mathf.Abs(directedDistance);
        Vector2 startPosition = target.MapPosition();
        Vector2 destination = startPosition;
        int collisionDamage = 0;
        for (int i = 0; i < distance; i++)
        {
            //evaluate each cell in turn as a push destination
            Vector2 possibleDestination = destination + direction;
            Vector2Int closestRealTile = possibleDestination.RoundToInt();
            GameObject cellObj = closestRealTile.TileAtMapPosition();
            if (cellObj != null)
            {
                if (cellObj.OccupyingUnit() == null)
                {
                    destination += direction;
                    continue;
                }
            }
            collisionDamage = distance - i;
            break;
        }
        BattleTileController destinationTile = destination.RoundToInt().TileAtMapPosition().GetComponent<BattleTileController>();
        MapTools.ReportPositionChange(target, destinationTile);
        yield return target.StartCoroutine(target.gameObject.LerpTo(destinationTile.unitPosition, duration));

        if (collisionDamage > 0 && directedDistance > 0)
        {
            GameObject impactPlace = (destination + direction).RoundToInt().TileAtMapPosition();
            if (impactPlace != null)
            {
                BattleUnit contents = impactPlace.OccupyingUnit();
                if (contents != null)
                {
                    Collide(contents, collisionDamage);
                }
            }
            Collide(target, collisionDamage);
            VFXMachine.PlayAtLocation("ImpactSmall", target.transform.position);
        }
    }

    void Collide(BattleUnit target, int factor)
    {
        float collisionDamage = .05f;
        target.ReceiveDamage(factor * Calcs.PercentMaxHealth(target, collisionDamage));
    }
}

