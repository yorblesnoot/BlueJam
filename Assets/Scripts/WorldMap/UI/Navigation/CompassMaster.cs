using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompassMaster : MonoBehaviour
{
    [SerializeField] List<WorldCompass> compasses;
    [SerializeField] RunData runData;
    public void DeployCompass(string searchKey, Color32 color)
    {
        Vector2Int targetLocation = runData.eventMap.FirstOrDefault(x => x.Value == searchKey).Key;
        WorldCompass compass = compasses.FirstOrDefault(x => x.gameObject.activeSelf == false);
        compass.gameObject.SetActive(true);
        compass.targetPosition = targetLocation;
        compass.gameObject.GetComponentInChildren<MeshRenderer>().material.color = color;
        compass.PointAtLockedCoordinates(MapTools.VectorToMap(WorldPlayerControl.player.transform.position) + WorldMapRenderer.spotlightGlobalOffset);
    }
}
