using System.Collections;
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
    public override void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        MonoBehaviour unitStats = actor.GetComponent<UnitActions>();
        unitStats.StartCoroutine(Push(actor, targetCell, pushDistance, stepSize));
    }

    IEnumerator Push(GameObject actor, GameObject targetCell, int distance, float stepsize)
    {
        Vector3 direction = new Vector3();
        direction = targetCell.transform.position - actor.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        direction.Normalize();
        Vector3 destination = targetCell.GetComponent<BattleTileController>().unitContents.transform.position;
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
        GameObject target = targetCell.GetComponent<BattleTileController>().unitContents;
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
                    Collide(impactPlace, collisionDamage);
                }
                Collide(targetCell, collisionDamage);
            }
        }
    }

    void Collide(GameObject cellTarget, int factor)
    {
        float collisionDamage = .05f;
        GameObject collided = cellTarget.GetComponent<BattleTileController>().unitContents;
        cellTarget.GetComponent<UnitActions>().ReceiveDamage(factor * Calcs.PercentMaxHealth(cellTarget, collisionDamage));
    }
}

