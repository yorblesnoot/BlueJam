using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerBattleState { IDLE, TARGETING_CARD, PERFORMING_ACTION, AWAITING_TURN }
public class PlayerUnit : BattleUnit
{
    public static PlayerBattleState playerState = PlayerBattleState.IDLE;
    public PlayerTurnIndicator turnIndicator;
    public readonly static int costPerGenericMove = 2;
    [SerializeField] BattleEnder ender;

    readonly float moveDuration = .3f;
    public override void Initialize()
    {
        unitStats = runData.playerStats;
        //pull current health from rundata
        base.Initialize();
        currentHealth = runData.currentHealth;
        myUI.InitializeHealth();
    }

    public override void TakeTurn()
    {
        base.TakeTurn();
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 2, "It's my turn again! The cards in my hand, below, give me access to powerful actions for defeating my enemies. Click one to select it!");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 4, "The blue bar above each enemy tells you how soon they'll take their turn. Their bars will fill up based on the number of pips on the top right of each card you use, or the number of tiles you move when clicking on the map.");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 5, "When hovering over a card or move, the orange zones by enemys' blue bars show how much their bars will fill when you act. White sparks mean the enemy will take a turn!");
        playerState = PlayerBattleState.IDLE;
    }

    public override void SpendBeats(int beats)
    {
        TurnManager.PlayerSpendBeats(beats);
    }

    public override void ModifyHealth(int reduction)
    {
        if (reduction == 0) return;
        Tutorial.Initiate(TutorialFor.BATTLEDAMAGE, TutorialFor.BATTLEACTIONS);
        Tutorial.EnterStage(TutorialFor.BATTLEDAMAGE, 1, "Ouch, I've taken damage! You can see my current health in bottom left. If it reaches zero, I'll die and lose all my progress!");
        base.ModifyHealth(reduction);
        runData.currentHealth = currentHealth;
    }

    public override void Die()
    {
        base.Die();
        //gameover
        isDead = true;
        TurnManager.deathPhase.RemoveListener(CheckForDeath);
        StartCoroutine(ender.DefeatSequence());
    }

    public IEnumerator ChainPath(List<GameObject> path)
    {
        playerState = PlayerBattleState.PERFORMING_ACTION;
        SpendBeats(path.Count * costPerGenericMove);
        VFXMachine.AttachTrail("MoveTrail", gameObject);
        MapTools.ReportPositionChange(this, path.Last().GetComponent<BattleTileController>());
        foreach (GameObject tile in path)
        {
            BattleTileController tileController = tile.GetComponent<BattleTileController>();
            transform.LookAt(tileController.unitPosition);
            yield return StartCoroutine(gameObject.LerpTo(tileController.unitPosition, moveDuration));
        }
        TurnManager.FinalizeTurn(this);
    }
}
