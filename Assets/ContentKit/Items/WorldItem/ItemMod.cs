using UnityEngine;

public abstract class ItemMod : ScriptableObject
{
    public float scaler;
    public abstract void ModifyPlayer(Unit player);
}
