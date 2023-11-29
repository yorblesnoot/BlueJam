using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiftMaker : MonoBehaviour
{
    [SerializeField] List<RiftType> types;
    [SerializeField] float riftHeight;
    public void PlaceRifts(Dictionary<Vector2Int, GameObject> map, int budget)
    {
        List<Vector2Int> riftedCells = new();
        while (budget > 0)
        {
            int riftSelect = Random.Range(0, types.Count);
            
            RiftType chosenRift = types[riftSelect];

            List<Vector2Int> cellsToCheck = map.Keys.ToList();
            while(cellsToCheck.Count > 0)
            {
                int cellSelect = Random.Range(0, cellsToCheck.Count);
                Vector2Int targetCell = cellsToCheck[cellSelect];
                List<Vector2Int> occupiedCells = RiftFits(targetCell, chosenRift, map);

                if (occupiedCells == null) continue;

                PlaceRift(chosenRift, targetCell);
                riftedCells.AddRange(occupiedCells);
                cellsToCheck.Remove(targetCell);
                budget -= chosenRift.Cost;
                break;
            }
        }

        //do something with the list of rift cells
    }

    private void PlaceRift(RiftType chosenRift, Vector2Int targetCell)
    {
        Instantiate(chosenRift.rift, targetCell.MapToVector(riftHeight), Quaternion.identity);
    }

    private List<Vector2Int> RiftFits(Vector2Int targetCell, RiftType chosenRift, Dictionary<Vector2Int, GameObject> map)
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
