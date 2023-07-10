using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectPush", menuName = "ScriptableObjects/CardEffects/Push")]
public class EffectPush : CardEffectPlus
{
    int pushes = 0;
    private void Reset()
    {
        effectClass = CardClass.ATTACK;
    }
    public int pushDistance;
    [Range(.1f, .01f)] public float stepSize;
    public override string GenerateDescription(IPlayerStats player)
    {
        return $"push target {pushDistance} cells";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
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
        direction = new Vector3(direction.x, 0f, direction.z);
        direction.Normalize();
        Vector3 destination = target.transform.position;
        int collisionDamage = 0;
        for (int i = 0; i < distance; i++)
        {
            //evaluate each cell in turn as a push destination
            Vector3 possibleDestination = destination + direction;
            BattleTileController cell;
            try
            {
                cell = MapTools.VectorToTile(possibleDestination).GetComponent<BattleTileController>();
                BattleUnit contents = cell.unitContents;
                if (contents == null)
                {
                    destination += direction;
                }
                else collisionDamage = distance - i;
            }
            catch { collisionDamage = distance - i; }

            if (collisionDamage > 0)
            {
                break;
            }
        }
        MapTools.ReportPositionChange(target, MapTools.VectorToTile(destination).GetComponent<BattleTileController>());
        while (target.transform.position != destination)
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, destination, stepsize);
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

