using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapKey", menuName = "ScriptableObjects/MapKey")]
public class MapKey : ScriptableObject
{
    // Start is called before the first frame update
    public Hashtable hashKey;
    public List<string> stringKey;
    public List<GameObject> objKey;

    public void Initialize()
    {
        hashKey = new Hashtable();
        for (int i = 0; i < stringKey.Count; i++)
        {
            hashKey.Add(stringKey[i],objKey[i]);
        }
    }
}
