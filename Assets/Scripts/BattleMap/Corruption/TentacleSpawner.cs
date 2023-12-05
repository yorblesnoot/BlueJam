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

    public override void Activate(Dictionary<Vector2Int, GameObject> battleMap, int amount)
    {
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
            Instantiate(tentacle, validSpots[selection].unitPosition, PhysicsHelper.RandomCardinalRotate());
            validSpots.RemoveAt(selection);
            amount--;
        }
    }
}
