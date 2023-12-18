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
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 2, "The other slimes are your foes! Click an enemy to learn more about it.");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 3, "It's your turn again! The cards in your hand, below, let you perform various actions. Click one to select it.");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 5, "The <color=blue>blue bar</color> above each enemy shows how soon it will take a turn. Each bar will fill up based on the <color=orange>number of pips</color> on the <color=orange>top right</color> of each card you use, or the <color=orange>number of tiles</color> you move when clicking on the map.");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 6, "When hovering over a card or move, the <color=orange>orange zones</color> by enemys' blue bars show how much their bars will fill when you act. <color=#00ffffff>White sparks</color> mean the enemy will take a turn!");
        Tutorial.EnterStage(TutorialFor.BATTLEACTIONS, 7, "The symbols above enemies show the types of cards they're holding. On their turn, an enemy will play a card from their hand. The cards they play then appear on the right.");
        playerState = PlayerBattleState.IDLE;
    }

    public override void SpendBeats(int beats)
    {
        TurnManager.PlayerSpendBeats(beats);
    }

    public override void ModifyHealth(int reduction)
    {
        if (reduction == 0) return;
        base.ModifyHealth(reduction);
        runData.currentHealth = currentHealth;
    }

    public override void Die()
    {
        base.Die();
        //gameover
        isDead = true;
        TurnManager.globalDeathCheck.RemoveListener(CheckForDeath);
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

        TurnManager.Main.StartCoroutine(TurnManager.EndTurn(this));
    }
}
