using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionElement : MonoBehaviour
{
    public abstract void Activate(Dictionary<Vector2Int, GameObject> map, int budget);
}
