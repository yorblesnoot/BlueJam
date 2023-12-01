using UnityEngine;

[CreateAssetMenu(fileName = "MapDispenser", menuName = "ScriptableObjects/Singleton/BattleMapDispenser")]
public class MapDispenser : ScriptableDictionary<TerrainType, GameObject> { }
