using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectPush", menuName = "ScriptableObjects/CardEffects/Push")]
public class EffectPush : CardEffectPlus
{
    public int pushDistance;
    [Range(.1f, .01f)] public float stepSize;
    public override string GenerateDescription()
    {
        description = $"Push the target {pushDistance}";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        List<BattleUnit> targets = base.Execute(actor, targetCell, aoe);
        MonoBehaviour unitStats = actor.GetComponent<BattleUnit>();
        foreach (BattleUnit target in targets)
            unitStats.StartCoroutine(Push(actor, target, pushDistance, stepSize));
        return targets;
    }

    IEnumerator Push(BattleUnit actor, BattleUnit target, int distance, float stepsize)
    {
        Vector3 direction = new Vector3();
        direction = target.transform.position - actor.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        direction.Normalize();
        Vector3 destination = target.transform.position;
        int collisionDamage = 0;
        for (int i = 1; i < distance; i++)
        {
            //evaluate each cell in turn as a push destination
            Vector3 possibleDestination = destination + direction;
            BattleTileController cell = GridTools.VectorToTile(possibleDestination).GetComponent<BattleTileController>();
            if (cell != null)
            {
                BattleUnit contents = cell.unitContents;
                if (contents == null)
                {
                    destination += direction;
                }
                else collisionDamage = distance - i;
            }
            else collisionDamage = distance - i;
            if (collisionDamage > 0)
            {
                break;
            }
        }
        GridTools.ReportPositionChange(target, GridTools.VectorToTile(destination).GetComponent<BattleTileController>());
        while (target.transform.position != destination)
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, destination, stepsize);
            yield return new WaitForSeconds(.01f);
        }

        if (collisionDamage > 0)
        {
            GameObject impactPlace = GridTools.VectorToTile(destination + direction);
            if (impactPlace != null)
            {
                BattleUnit contents = impactPlace.GetComponent<BattleTileController>().unitContents;
                if (contents != null)
                {
                    Collide(contents, collisionDamage);
                }
                Collide(target, collisionDamage);
            }
        }
    }

    void Collide(BattleUnit target, int factor)
    {
        float collisionDamage = .05f;
        target.ReceiveDamage(factor * Calcs.PercentMaxHealth(target, collisionDamage));
    }
}

