using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleLauncher : MapLauncher
{
    public GameObject player;
    public SceneRelay sceneRelay;

    [SerializeField] CameraLock camLock;
    [SerializeField] MasterEnemyPool masterEnemyPool;

    [SerializeField] CorruptionManager corruptionManager;

    [SerializeField] MapDispenser dispenser;

    private void Start()
    {
        //instantiate a prefab map
        Instantiate(dispenser[sceneRelay.battleMap], new Vector3(0, 0, 0), Quaternion.identity);

        //tell the camera to find the lockpoint on the battle map and lock onto it
        camLock.CameraLockOn();
        RequestMapReferences();
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        corruptionManager.CorruptScene();

        //place units onto the map
        BattleUnitSpawner encounterBuilder = new(sceneRelay.spawnPool, map, masterEnemyPool);
        
        Tutorial.Initiate(TutorialFor.BATTLEACTIONS, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 1, "The fight is on! An <color=orange>orange exclamation</color> over me means it's my turn. On my turn, you can click on a map tile and I'll move there.");
        
        if (sceneRelay.bossEncounter == true) encounterBuilder.PlaceBoss(runData.bossSequence);
        else encounterBuilder.SmartSpawn(sceneRelay.enemyBudget);

        yield return StartCoroutine(encounterBuilder.PlacePlayer(player));

        PlayerUnit playerUnit = player.GetComponent<PlayerUnit>();
        TurnManager.InitializePositions();

        //activate item effects
        foreach (BattleItem item in runData.itemInventory.OfType<BattleItem>())
        {
            foreach (var effect in item.effects)
            {
                effect.Initialize();
                StartCoroutine(effect.Execute(playerUnit, MapTools.VectorToTile(player.transform.position).GetComponent<BattleTileController>()));
            }
        }

        //initialize combat
        TurnManager.InitializeTurns();
        if(sceneRelay.bossEncounter == true)
            SoundManager.PlayMusic(SoundType.MUSICBOSS);
        else SoundManager.PlayMusic(SoundType.MUSICBATTLE);
    }
}
