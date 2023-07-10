using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerBattleState { IDLE, TARGETING_CARD, PERFORMING_ACTION, AWAITING_TURN }
public class PlayerUnit : BattleUnit
{
    public static PlayerBattleState playerState = PlayerBattleState.IDLE;
    public PlayerTurnIndicator turnIndicator;
    public static int costPerGenericMove = 2;
    [SerializeField] BattleEnder ender;
    public override void Initialize()
    {
        unitStats = runData.playerStats;
        //pull current health from rundata
        base.Initialize();
        currentHealth = runData.currentHealth;
        myUI.InitializeHealth();
    }

    public override void GetAction()
    {
        playerState = PlayerBattleState.IDLE;
    }

    public override void ReduceHealth(int reduction)
    {
        base.ReduceHealth(reduction);
        runData.currentHealth = currentHealth;
    }

    public override void Die()
    {
        //gameover
        isDead = true;
        TurnManager.deathPhase.RemoveListener(CheckForDeath);
        StartCoroutine(ender.DefeatSequence());
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        playerState = PlayerBattleState.PERFORMING_ACTION;
        VFXMachine.AttachTrail("MoveTrail", gameObject);
        MapTools.ReportPositionChange(this, path.Last().GetComponent<BattleTileController>());
        foreach (GameObject tile in path)
        {
            BattleTileController tileController = tile.GetComponent<BattleTileController>();
            transform.LookAt(tileController.unitPosition);
            while (gameObject.transform.position != tileController.unitPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, tileController.unitPosition, .05f);
                yield return new WaitForSeconds(.01f);
            }
        }
        TurnManager.SpendBeats(this, path.Count * costPerGenericMove);
    }
}
