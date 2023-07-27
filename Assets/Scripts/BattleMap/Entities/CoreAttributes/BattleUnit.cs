using System.Collections.Generic;
using UnityEngine;

public enum StatType { MAXHEALTH, HANDSIZE, SPEED, BEATS, DAMAGE, HEAL, BARRIER }
public class BattleUnit : MonoBehaviour, IUnitStats
{
    [field: SerializeField] public UnitStats unitStats { get; set; }
    [SerializeField] BarrierTracker barrierTracker;
    public BuffTracker buffTracker;
    public HandPlus myHand;
    public UnitAnimator unitAnimator;

    public Dictionary<StatType, float> loadedStats { get; set; }
    public int currentHealth { get; set; }
    public int deflectHealth { get; set; }
    public int shieldHealth { get; set; }

    public bool isSummoned;

    public bool isDead;

    public RunData runData;

    [HideInInspector]public EntityUI myUI { get; set; }

    void Awake()
    {
        Initialize();
        TurnManager.deathPhase.AddListener(CheckForDeath);
    }

    public virtual void TakeTurn()
    {
        barrierTracker.DeflectLapse();
    }

    public virtual void Initialize()
    {
        loadedStats = new()
        {
            { StatType.MAXHEALTH, unitStats.maxHealth },
            { StatType.DAMAGE, unitStats.damageScaling },
            { StatType.HEAL, unitStats.healScaling },
            { StatType.BARRIER, unitStats.barrierScaling },
            { StatType.HANDSIZE, unitStats.handSize },
            { StatType.SPEED, unitStats.turnSpeed },
            { StatType.BEATS, unitStats.startBeats },
        };
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
