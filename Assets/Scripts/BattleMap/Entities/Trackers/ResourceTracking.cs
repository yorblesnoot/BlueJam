using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker
{
    Dictionary<string, int> resourceCount;
    public void AddResource(string resource)
    {
        resourceCount ??= new Dictionary<string, int>();
        if(!resourceCount.ContainsKey(resource)) resourceCount.Add(resource, 1);
        else resourceCount[resource]++;
    }

    public int GetResource(string resource)
    {
        if(resourceCount == null) return 0;
        return resourceCount[resource];
    }
}
