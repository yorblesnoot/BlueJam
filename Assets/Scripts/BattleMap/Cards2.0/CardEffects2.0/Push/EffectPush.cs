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
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        List<GameObject> targets = base.Execute(actor, targetCell, aoe);
        MonoBehaviour unitStats = actor.GetComponent<BattleUnit>();
        foreach (GameObject target in targets)
            unitStats.StartCoroutine(Push(actor, target, pushDistance, stepSize));
        return targets;
    }

    IEnumerator Push(GameObject actor, GameObject target, int distance, float stepsize)
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
            GameObject cell = GridTools.VectorToTile(possibleDestination);
            if (cell != null)
            {
                GameObject contents = cell.GetComponent<BattleTileController>().unitContents;
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
        GridTools.ReportPositionChange(target, GridTools.VectorToTile(destination));
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
                GameObject contents = impactPlace.GetComponent<BattleTileController>().unitContents;
                if (contents != null)
                {
                    Collide(contents, collisionDamage);
                }
                Collide(target, collisionDamage);
            }
        }
    }

    void Collide(GameObject target, int factor)
    {
        float collisionDamage = .05f;
        target.GetComponent<BattleUnit>().ReceiveDamage(factor * Calcs.PercentMaxHealth(target, collisionDamage));
    }
}

