using UnityEngine;

public enum VFXStyle
{
    UNIT,
    ATTRACT,
    DIRECTION,
    TRAIL,
    AURA,
    DROP,
    SELFALWAYS
}
public static class VFXMachine 
{
    static readonly float particleKillDelay = 4f;
    public static void PlayVFX(string vfxName, VFXStyle vfxStyle, BattleUnit actor, BattleTileController targetCell)
    {
        if (vfxName == "none" || string.IsNullOrEmpty(vfxName)) return;
        switch (vfxStyle)
        {
            case (VFXStyle.UNIT): PlayAtLocation(vfxName, targetCell.unitPosition); break;
            case (VFXStyle.ATTRACT): ShootToLocation(vfxName, actor.gameObject.transform.position, targetCell.unitPosition); break;
            case (VFXStyle.DIRECTION): PlayToLocation(vfxName, actor.gameObject.transform.position, targetCell.unitPosition); break;
            case (VFXStyle.TRAIL): AttachTrail(vfxName, actor.gameObject); break;
            case (VFXStyle.DROP): PlayAboveLocation(vfxName, targetCell.unitPosition); break;
            case (VFXStyle.SELFALWAYS): PlayAtLocation(vfxName, actor.transform.position); break;

        }
    }
    public static void PlayToLocation(string effect, Vector3 origin, Vector3 target)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        particle.transform.LookAt(target);
        GameObject.Destroy(particle, particleKillDelay);
    }

    public static void PlayAtLocation(string effect, Vector3 origin)
    {   
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        GameObject.Destroy(particle, particleKillDelay);
    }

    public static void PlayAboveLocation(string effect, Vector3 origin)
    {
        Vector3 above = new(origin.x, origin.y + 1f, origin.z);
        PlayAtLocation(effect, above);
    }

    public static void ShootToLocation(string effect, Vector3 origin, Vector3 target)
    {


    }

    public static void AttachTrail(string effect, GameObject attached)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), attached.transform.position, Quaternion.identity , attached.transform);
        GameObject.Destroy(particle, particleKillDelay);
    }

    static GameObject PrepareAsset(string effect)
    {
        string assetFolder = "VFX/";
        effect = assetFolder + effect;
        GameObject prefab = (GameObject)Resources.Load(effect, typeof(GameObject));
        return prefab;
    }
}
