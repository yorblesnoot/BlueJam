using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectPushPull", menuName = "ScriptableObjects/CardEffects/PushPull")]
public class EffectPushPull : CardEffectPlus
{
    int pushes = 0;
    private void Reset()
    {
        effectClass = CardClass.ATTACK;
    }
    [Range(.1f, .01f)] public float stepSize;
    public override string GenerateDescription(IPlayerStats player)
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
            target.StartCoroutine(Push(actor, target, pushDistance, stepSize));
            pushes++;
        }
        yield return new WaitUntil(() => pushes == 0);
    }

    IEnumerator Push(BattleUnit actor, BattleUnit target, int distance, float stepsize)
    {
        Vector3 direction;
        direction = target.transform.position - actor.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z)*distance;
        direction.Normalize();
        distance = Mathf.Abs(distance);
        Vector3 destination = target.transform.position;
        int collisionDamage = 0;
        for (int i = 0; i < distance; i++)
        {
            //evaluate each cell in turn as a push destination
            Vector3 possibleDestination = destination + direction;
            BattleTileController cell;
            GameObject cellObj = MapTools.VectorToTile(possibleDestination);
            if(cellObj != null)
            {
                cell = cellObj.GetComponent<BattleTileController>();
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
        while (target.transform.position != destinationTile.unitPosition)
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, destinationTile.unitPosition, stepsize);
            yield return new WaitForSeconds(.01f);
        }

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
        pushes--;
    }

    void Collide(BattleUnit target, int factor)
    {
        float collisionDamage = .05f;
        target.ReceiveDamage(factor * Calcs.PercentMaxHealth(target, collisionDamage));
    }
}

