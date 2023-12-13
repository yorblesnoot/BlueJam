using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiftMaker : CorruptionElement
{
    [SerializeField] List<RiftType> types;
    public static List<Vector2Int> riftedCells;
    List<RiftType> loadedTypes;
    private void Awake()
    {
        loadedTypes = new(types);
    }
    public override void Activate(int budget)
    {
        
        riftedCells = new();
        Dictionary<Vector2Int, GameObject> availableMap = new(MapTools.gameMap);
        while (budget > 0)
        {
            if (loadedTypes.Count == 0) { Debug.LogWarning("Ran out of rift space."); break; }
            int riftSelect = Random.Range(0, loadedTypes.Count);
            
            RiftType chosenRift = loadedTypes[riftSelect];

            List<Vector2Int> cellsToCheck = availableMap.Keys.ToList();
            while(cellsToCheck.Count > 0)
            {
                int cellSelect = Random.Range(0, cellsToCheck.Count);
                Vector2Int targetCell = cellsToCheck[cellSelect];
                cellsToCheck.Remove(targetCell);
                List<Vector2Int> occupiedCells = RiftBlueprint(targetCell, chosenRift, availableMap);

                if (occupiedCells == null) continue;
                if (RiftCrowdingOtherRifts(occupiedCells, riftedCells)) continue;
                if (RiftWouldBlockPathing(occupiedCells, availableMap)) continue;
                

                RemoveCellsFromMap(occupiedCells, availableMap);
                PlaceRift(chosenRift, targetCell, occupiedCells);
                budget -= chosenRift.Cost;

                riftedCells.AddRange(occupiedCells);
                break;
            }
            if (cellsToCheck.Count == 0) loadedTypes.Remove(chosenRift);
        }

        //do something with the list of rift cells
        foreach(var cell in riftedCells)
        {
            cell.MapToTile().GetComponent<BattleTileController>().BecomeRift();
        }
    }

    private bool RiftCrowdingOtherRifts(List<Vector2Int> occupiedCells, List<Vector2Int> riftedCells)
    {
        foreach(var cell in occupiedCells)
        {
            List<Vector2Int> adjacents = cell.GetAdjacentCoordinates();
            foreach(var adjacent in adjacents)
            {
                if (riftedCells.Contains(adjacent)) return true;
            }
        }
        return false;
    }

    private void RemoveCellsFromMap(List<Vector2Int> occupiedCells, Dictionary<Vector2Int, GameObject> availableMap)
    {
        foreach(var cell in occupiedCells)
        {
            availableMap.Remove(cell);
        }
    }

    private bool RiftWouldBlockPathing(List<Vector2Int> occupiedCells, Dictionary<Vector2Int, GameObject> availableMap)
    {
        Dictionary<Vector2Int, GameObject> potentialMap = new(availableMap);
        RemoveCellsFromMap(occupiedCells, potentialMap);

        Vector2Int startPoint = potentialMap.Keys.First();
        FloodRecursive(startPoint);

        if (potentialMap.Count == 0) return false;
        else return true;

        void FloodRecursive(Vector2Int origin)
        {
            List<Vector2Int> surrounding = origin.GetAdjacentCoordinates();
            potentialMap.Remove(origin);
            foreach (Vector2Int x in surrounding)
            {
                if (potentialMap.ContainsKey(x))
                {
                    FloodRecursive(x);
                }
            }
        }
    }

    private void PlaceRift(RiftType chosenRift, Vector2Int targetCell, List<Vector2Int> occupiedCells)
    {
        Vector3 placementPosition = targetCell.MapToTile().transform.position;

        float targetZ = occupiedCells.Select(coord => coord.MapToTile().transform.position.z).Min();
        placementPosition.z = targetZ;
        
        Instantiate(chosenRift.rift, targetCell.MapToTile().transform.position, Quaternion.identity);
    }

    private List<Vector2Int> RiftBlueprint(Vector2Int targetCell, RiftType chosenRift, Dictionary<Vector2Int, GameObject> map)
    {
        List<Vector2Int> output = new();
        for (int x =  0; x < chosenRift.dimensions.x; x++)
        {
            for(int y = 0; y < chosenRift.dimensions.y; y++)
            {
                Vector2Int checkedCell = targetCell + new Vector2Int(x, y);
                if (!map.ContainsKey(checkedCell)) return null;
                output.Add(checkedCell);
            }
        }
        return output;
    }

    [System.Serializable]
    class RiftType
    {
        public GameObject rift;
        public Vector2Int dimensions;
        public int Cost { get {  return dimensions.x * dimensions.y; } }
    }
}
