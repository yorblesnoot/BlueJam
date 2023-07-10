using UnityEngine;

public class BattleLauncher : MapLauncher
{
    public GameObject player;
    public SceneRelay sceneRelay;
    public RunData runData;

    [SerializeField] CameraLock camLock;
    [SerializeField] MasterEnemyPool masterEnemyPool;

    private void Start() 
    {
        //instantiate a prefab map
        Instantiate(sceneRelay.availableMaps.DispenseMap(), new Vector3(0, 0, 0), Quaternion.identity);

        //tell the camera to find the lockpoint on the battle map and lock onto it
        camLock.CameraLockOn();
        RequestMapReferences();

        //place units onto the map
        BattleUnitSpawner encounterBuilder = new(sceneRelay.spawnPool, map, masterEnemyPool);
        encounterBuilder.PlacePlayer(player);
        PlayerUnit playerUnit = player.GetComponent<PlayerUnit>();
        encounterBuilder.PlaceEnemies(sceneRelay.enemyBudget);

        //initialize combat
        TurnManager.InitializeTurns();

        //activate item effects
        foreach (BattleItem item in runData.itemInventory)
        {
            foreach (var effect in item.effects)
            {
                effect.Initialize();
                StartCoroutine(effect.Execute(playerUnit, MapTools.VectorToTile(player.transform.position).GetComponent<BattleTileController>()));
            }
        }
    }
}
