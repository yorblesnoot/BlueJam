using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    readonly GameObject pooled;
    List<GameObject> inactivePool;
    public ObjectPool(GameObject toPool)
    {
        pooled = toPool;
        inactivePool = new();
    }

    public GameObject InstantiateFromPool(Vector3 position, Quaternion rotation)
    {
        if(inactivePool.Count == 0) 
            return GameObject.Instantiate(pooled, position, rotation);
        else
        {
            GameObject output = inactivePool[0];
            output.transform.SetPositionAndRotation(position, rotation);
            output.SetActive(true);
            
            inactivePool.RemoveAt(0);
            return output;
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        inactivePool.Add(obj);
    }
}