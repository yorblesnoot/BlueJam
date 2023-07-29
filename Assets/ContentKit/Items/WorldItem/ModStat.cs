using UnityEngine;

[CreateAssetMenu(fileName = "ModStat", menuName = "ScriptableObjects/ItemMods/Stat")]
public class ModStat : ItemMod
{
    [SerializeField] StatType statType;
    public override void ModifyPlayer(Unit p)
    {
        Stat(scaler, p);
    }
    void Stat(float scale, Unit target)
    {
        if (statType == StatType.HANDSIZE || statType == StatType.BEATS)
            target.loadedStats[statType] += scale;
        else target.loadedStats[statType] *= 1 + scale / 100;
    }
}
