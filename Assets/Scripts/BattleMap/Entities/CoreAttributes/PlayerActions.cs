using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActions : BattleUnit
{
    public override void Initialize()
    {
        unitStats = runData.playerStats;
        //pull current health from rundata
        base.Initialize();
        currentHealth = runData.currentHealth;
    }

    public override void ReduceHealth(int reduction)
    {
        currentHealth -= reduction;
        runData.currentHealth = currentHealth;
    }

    public override void Die()
    {
        //gameover
        SceneManager.LoadScene(0);
    }
}
