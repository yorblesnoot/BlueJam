using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUnit : BattleUnit
{
    public override void Initialize()
    {
        unitStats = runData.playerStats;
        //pull current health from rundata
        base.Initialize();
        currentHealth = runData.currentHealth;
        myUI.InitializeHealth();
    }

    public override void ReduceHealth(int reduction)
    {
        base.ReduceHealth(reduction);
        runData.currentHealth = currentHealth;
    }

    public override void Die()
    {
        //gameover
        System.IO.File.Delete(Application.persistentDataPath + "/runData.json");
        SceneManager.LoadScene(0);
    }
}
