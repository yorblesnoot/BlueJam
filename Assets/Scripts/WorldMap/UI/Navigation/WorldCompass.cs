using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCompass : MonoBehaviour
{
    readonly int distanceFromCenter = 5;
    public void PointAtWorldCoordinates(Vector2Int globalTargetCoords, Vector3 playerLocation)
    {
        Vector2Int playerGlobalMap = MapTools.VectorToMap(playerLocation);
        Vector2 direction = globalTargetCoords - playerGlobalMap;
        direction.Normalize();
        direction *= distanceFromCenter;
        StopAllCoroutines();
        StartCoroutine(GradualPoint(new Vector3(playerLocation.x + direction.x, playerLocation.y, playerLocation.z + direction.y),
            MapTools.MapToVector(globalTargetCoords, playerLocation.y)));
    }

    IEnumerator GradualPoint(Vector3 location, Vector3 lookTarget)
    {
        transform.LookAt(lookTarget);
        while(transform.position != location) 
        {
            transform.position = Vector3.MoveTowards(transform.position, location, .1f);
            yield return new WaitForSeconds(.1f);
        }
    }
}
