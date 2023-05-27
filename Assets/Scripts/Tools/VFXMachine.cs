using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public static class VFXMachine 
{
    public static void PlayVFX(string vfxName, VFXStyle vfxStyle, GameObject actor, GameObject targetCell)
    {
        BattleTileController cellControl = targetCell.GetComponent<BattleTileController>();
        switch (vfxStyle)
        {
            case (VFXStyle.UNIT): PlayAtLocation(vfxName, cellControl.unitPosition); break;
            case (VFXStyle.DIRECTION): PlayToLocation(vfxName, actor.transform.position, cellControl.unitPosition); break;
            case (VFXStyle.TRAIL): AttachTrail(vfxName, actor); break;

        }
    }
    public static void PlayToLocation(string effect, Vector3 origin, Vector3 target)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        particle.transform.LookAt(target);
        GameObject.Destroy(particle, 2f);
    }

    public static void PlayAtLocation(string effect, Vector3 origin)
    {   
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), origin, Quaternion.identity);
        GameObject.Destroy(particle, 2f);
    }

    public static void ShootToLocation()
    {

    }

    public static void AttachTrail(string effect, GameObject attached)
    {
        GameObject particle = GameObject.Instantiate(PrepareAsset(effect), attached.transform.position, Quaternion.identity , attached.transform);
        GameObject.Destroy(particle, 2f);
    }

    static GameObject PrepareAsset(string effect)
    {
        string assetFolder = "Assets/VFX/";
        effect = assetFolder + effect + ".prefab";
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(effect, typeof(GameObject));
        return prefab;
    }
}
