using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectPushPull", menuName = "ScriptableObjects/CardEffects/PushPull")]
public class EffectPushPull : CardEffectPlus
{
    int pushes;
    private void Reset()
    {
        effectClass = CardClass.ATTACK;
        steps = 20;
    }
    [Range(20, 50)] public int steps;
    public override string GenerateDescription(IPlayerStats player)
    {
        string verb;
        if (scalingMultiplier > 0) verb = "push";
        else verb = "pull";
        return $"{verb} target {Mathf.Abs(Mathf.RoundToInt(scalingMultiplier))} cells";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        pushes = 0;
        int pushDistance = Mathf.RoundToInt(scalingMultiplier);
        foreach (BattleUnit target in targets)
        {
            Debug.Log(target.name);
            target.StartCoroutine(Push(actor, target, pushDistance, steps));
            pushes++;
        }
        Debug.Log(pushes);
        yield return new WaitUntil(() => pushes == 0);
    }

    IEnumerator Push(BattleUnit actor, BattleUnit target, int distance, int steps)
    {
        Vector3 direction;
        direction = target.transform.position - actor.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z)*distance;
        direction.Normalize();
        distance = Mathf.Abs(distance);
        Vector3 startPosition = target.transform.position;
        Vector3 destination = startPosition;
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
        for (int i = 0; i < steps; i++)
        {
            target.transform.position = Vector3.Lerp(startPosition, destinationTile.unitPosition, (float)i / steps);
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

