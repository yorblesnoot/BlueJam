using System.Collections.Generic;
using UnityEngine;

public class MapLauncher : MonoBehaviour
{
    public List<GameObject> map = new();
    [SerializeField] internal RunData runData;

    private void Awake()
    {
        Random.state = runData.randomState;
    }
    public virtual void RequestMapReferences()
    {
        EventManager.requestMapReferences.Invoke(this);
        MapTools.SubmitTileMap(map);
    }

    public void SubmitMapReference(GameObject reference)
    {
        map.Add(reference);
    }
}
