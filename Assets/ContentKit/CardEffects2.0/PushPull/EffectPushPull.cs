using System.Collections;
using System.Collections.Generic;
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
        foreach (BattleUnit target in targets)
        {
            yield return target.StartCoroutine(Push(actor, target, pushDistance, duration));
        }
    }

    IEnumerator Push(BattleUnit actor, BattleUnit target, int distance, float duration)
    {
        Vector3 direction;
        direction = target.transform.position - actor.transform.position;
        direction.y = 0f;
        direction *= distance;
        direction.Normalize();
        distance = Mathf.Abs(distance);
        Vector3 startPosition = target.transform.position;
        Vector3 destination = startPosition;
        int collisionDamage = 0;
        for (int i = 0; i < distance; i++)
        {
            //evaluate each cell in turn as a push destination
            Vector3 possibleDestination = destination + direction;
            GameObject cellObj = MapTools.VectorToTile(possibleDestination);
            if(cellObj != null)
            {
                BattleTileController cell = cellObj.GetComponent<BattleTileController>();
                if (cell.unitContents == null)
                {
                    destination += direction;
                    continue;
                }
            }
            collisionDamage = distance - i;
            break;
        }
        BattleTileController destinationTile = MapTools.VectorToTile(destination).GetComponent<BattleTileController>();
        MapTools.ReportPositionChange(target, destinationTile);
        yield return target.StartCoroutine(target.gameObject.LerpTo(destinationTile.unitPosition, duration));

        if (collisionDamage > 0)
        {
            GameObject impactPlace = MapTools.VectorToTile(destination + direction);
            if (impactPlace != null)
            {
                BattleUnit contents = impactPlace.GetComponent<BattleTileController>().unitContents;
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

