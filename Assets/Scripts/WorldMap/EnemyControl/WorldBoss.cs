using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoss : WorldEnemy
{
    public SpawnPool bossPool;
    private void OnEnable()
    {
        Tutorial.Initiate(TutorialFor.WORLDBOSS, TutorialFor.MAIN);
        Tutorial.EnterStage(TutorialFor.WORLDBOSS, 1, "That big skull over there is the boss! Move me onto its tile to battle, but make sure I'm ready for a tough fight first!");
    }
    public override void PullSpawnPool(WorldEventHandler handler)
    {
        //get static spawn
        spawnPool = bossPool;
    }
}
