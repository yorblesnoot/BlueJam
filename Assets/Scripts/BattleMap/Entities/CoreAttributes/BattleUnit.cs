using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour, IPlayerStats
{
    [field: SerializeField] public UnitStats unitStats { get; set; }
    [SerializeField] BarrierTracker barrierTracker;
    [SerializeField] BuffTracker buffTracker;
    [SerializeField] Hand myHand;
    public UnitAnimator unitAnimator;

    public int maxHealth { get; set; }
    public int handSize { get; set; }
    public float turnSpeed { get; set; }

    public int currentHealth { get; set; }
    public int deflectHealth { get; set; }
    public int shieldHealth { get; set; }

    public float currentBeats { get; set; }

    public int damageScaling { get; set; }
    public int healScaling { get; set; }
    public int barrierScaling { get; set; }

    public bool isSummoned;

    public bool isDead;

    public RunData runData;

    [HideInInspector]public EntityUI myUI { get; set; }

    void Awake()
    {
        Initialize();
        TurnManager.deathPhase.AddListener(CheckForDeath);
    }

    public void TakeTurn()
    {
        //buff lapse effects DurationProc()
        buffTracker.DurationProc();
        TurnManager.deathPhase?.Invoke();
        if (isDead)
        {
            TurnManager.AssignTurn();
            return;
        }
        myHand.DrawPhase();

        GetAction();
        //check for deaths
        //spend beats -> next turn
    }

    public virtual void GetAction() { }

    public virtual void Initialize()
    {
        maxHealth = unitStats.maxHealth;
        damageScaling = unitStats.damageScaling;
        healScaling = unitStats.healScaling;
        barrierScaling = unitStats.barrierScaling;
        handSize = unitStats.handSize;
        turnSpeed = unitStats.turnSpeed;
        currentBeats = unitStats.startBeats;
        TurnManager.initialPositionReport.AddListener(ReportCell);
        myUI = GetComponentInChildren<EntityUI>();
    }

    public void ReceiveDamage(int damage)
    {
        if (deflectHealth > 0) damage = barrierTracker.ReceiveDeflectDamage(damage);
        if (shieldHealth > 0) damage = barrierTracker.ReceiveShieldDamage(damage);

        ReduceHealth(damage);
        //update healthbar in UI
        unitAnimator.Animate(AnimType.DAMAGED);
    }

    public void CheckForDeath()
    {
        if (currentHealth <= 0) Die();
    }

    public virtual void ReduceHealth(int reduction)
    {
        currentHealth -= reduction;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        myUI.UpdateHealth();
    }

    public void ReportCell()
    {
        //report our location to the cell we're in
        GameObject myTile = MapTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = this;
    }

    public void UnreportCell()
    {
        //report leaving a cell
        GameObject myTile = MapTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = null;
    }

    public virtual void Die() { }
}
