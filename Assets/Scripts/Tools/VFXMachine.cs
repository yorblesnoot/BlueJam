using UnityEngine;

public enum VFXStyle
{
    UNIT,
    ARC,
    DIRECTION,
    TRAIL,
    AURA,
    DROP,
    SELFALWAYS
}
public static class VFXMachine 
{
    static readonly float particleKillDelay = 6f;
    public static void PlayVFX(string vfxName, VFXStyle vfxStyle, BattleUnit actor, BattleTileController targetCell)
    {
        if (vfxName == "none" || string.IsNullOrEmpty(vfxName)) return;
        switch (vfxStyle)
        {
            case (VFXStyle.UNIT): PlayAtLocation(vfxName, targetCell.unitPosition); break;
            case (VFXStyle.ARC): ShootToLocation(vfxName, actor.gameObject.transform.position, targetCell.unitPosition); break;
            case (VFXStyle.DIRECTION): PlayToLocation(vfxName, actor.gameObject.transform.position, targetCell.unitPosition); break;
            case (VFXStyle.TRAIL): AttachTrail(vfxName, actor.gameObject); break;
            case (VFXStyle.DROP): PlayAboveLocation(vfxName, targetCell.unitPosition); break;
            case (VFXStyle.SELFALWAYS): PlayAtLocation(vfxName, actor.transform.position); break;

        }
    }
    public static void PlayToLocation(string effect, Vector3 origin, Vector3 target)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        StopParticlesFromLooping(particle);
        particle.transform.LookAt(target);
        GameObject.Destroy(particle, particleKillDelay);
    }

    public static void PlayAtLocation(string effect, Vector3 origin)
    {   
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        StopParticlesFromLooping(particle);
        GameObject.Destroy(particle, particleKillDelay);
    }

    public static void PlayAboveLocation(string effect, Vector3 origin)
    {
        Vector3 above = new(origin.x, origin.y + 1f, origin.z);
        PlayAtLocation(effect, above);
    }

    readonly static float shootTime = 1f;
    public static void ShootToLocation(string effect, Vector3 origin, Vector3 target)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        target.y += .1f;
        StopParticlesFromLooping(particle);
        particle.ParabolicProjectile(target, shootTime);
        GameObject.Destroy(particle, particleKillDelay);
    }

    readonly static float trailDelay = 1f;
    public static void AttachTrail(string effect, GameObject attached)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), attached.transform.position, Quaternion.identity , attached.transform);
        GameObject.Destroy(particle, trailDelay);
    }

    static GameObject PrepareAsset(string effect)
    {
        string assetFolder = "VFX/";
        effect = assetFolder + effect;
        GameObject prefab = (GameObject)Resources.Load(effect, typeof(GameObject));
        return prefab;
    }

    static void StopParticlesFromLooping(GameObject parent)
    {
        ParticleSystem[] particles = parent.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule main = particle.main;
            main.loop = false;
        }
    }
}
