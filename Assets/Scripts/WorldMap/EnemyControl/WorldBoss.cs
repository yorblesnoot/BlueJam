using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoss : WorldEnemy
{
    public SpawnPool bossPool;
    public override void PullSpawnPool(WorldEventHandler handler)
    {
        Tutorial.Initiate(TutorialFor.WORLDBOSS, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.WORLDBOSS, 1, "That skull over there is the boss! Move me onto its tile once I'm ready for a tough fight!");
        //get static spawn
        spawnPool = bossPool;
    }
}
