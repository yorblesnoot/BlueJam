using System.Collections.Generic;
using UnityEngine;


public class BattleUnit : Unit
{
    [SerializeField] BarrierTracker barrierTracker;
    public BuffTracker buffTracker;
    public HandPlus myHand;
    public UnitAnimator unitAnimator;

    public int deflectHealth { get; set; }
    public int shieldHealth { get; set; }

    [HideInInspector] public bool isSummoned;
    [HideInInspector] public bool isDead;
    [HideInInspector]public EntityUI myUI { get; set; }

    void Awake()
    {
        Initialize(); 
    }

    public virtual void TakeTurn()
    {
        barrierTracker.DeflectLapse();
    }

    public virtual void Initialize()
    {
        LoadStats();
        TurnManager.deathPhase.AddListener(CheckForDeath);
        TurnManager.initialPositionReport.AddListener(ReportCell);
        myUI = GetComponentInChildren<EntityUI>();
    }

    public void ReceiveDamage(int damage)
    {
        if (deflectHealth > 0) damage = barrierTracker.ReceiveDeflectDamage(damage);
        if (shieldHealth > 0) damage = barrierTracker.ReceiveShieldDamage(damage);

        ReduceHealth(damage);
        unitAnimator.Animate(AnimType.DAMAGED);
    }

    public void CheckForDeath()
    {
        if (currentHealth <= 0) Die();
    }

    public virtual void ReduceHealth(int reduction)
    {
        currentHealth -= reduction;
        if (currentHealth > loadedStats[StatType.MAXHEALTH]) currentHealth = Mathf.RoundToInt(loadedStats[StatType.MAXHEALTH]);
        myUI.UpdateHealth();
    }

    public void ReportCell()
    {
        GameObject myTile = MapTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = this;
    }

    public void UnreportCell()
    {
        GameObject myTile = MapTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = null;
    }

    public virtual void Die()
    {
        SoundManager.PlaySound(SoundTypeEffect.DIE);
    }
}
