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
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 2, "It's my turn again! The cards in my hand, below, give me access to powerful actions for defeating my enemies. Click one to select it!");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 4, "See the blue bar above each enemy? That tells you how soon they'll take their turn! Their bars will fill up based on your actions.");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 5, "When hovering over a card or move, the orange zones by enemys' blue bars show how much their bars will fill when you act. White sparks mean the enemy will take a turn!");
        playerState = PlayerBattleState.IDLE;
    }

    public override void ReduceHealth(int reduction)
    {
        if (reduction == 0) return;
        Tutorial.Initiate(TutorialFor.BATTLEDAMAGE, TutorialFor.BATTLEACTIONS);
        Tutorial.EnterStage(TutorialFor.BATTLEDAMAGE, 1, "Ouch, I've taken damage! You can see my health in the green bar on the right. If it empties fully, I'll die and lose all my progress!");
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
