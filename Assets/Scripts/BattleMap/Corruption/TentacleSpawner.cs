using System.Collections.Generic;
using UnityEngine;

public class TentacleSpawner : CorruptionElement
{
    [SerializeField] GameObject tentacle;
    List<BattleTileController> validSpots;

    private void Awake()
    {
        validSpots = new();
    }

    public override void Activate(int amount)
    {
        Dictionary<Vector2Int, GameObject> battleMap = MapTools.tileMap.forward;
        foreach (Vector2Int key in battleMap.Keys)
        {
            GameObject tile = battleMap[key];
            if (tile != null)
            {
                BattleTileController battleTileController = tile.GetComponent<BattleTileController>();
                if (battleTileController.IsRift) validSpots.Add(battleTileController);
            }
        }
        while (amount > 0)
        {
            int selection = Random.Range(0, validSpots.Count);
            GameObject spawned = Instantiate(tentacle, validSpots[selection].unitPosition, PhysicsHelper.RandomCardinalRotate());
            MapTools.ReportPositionChange(spawned.GetComponent<BattleUnit>(), validSpots[selection]);
            validSpots.RemoveAt(selection);
            amount--;
        }
    }
}
