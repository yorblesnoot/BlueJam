using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour, IPlayerData
{
    [field: SerializeField] public UnitStats unitStats { get; set; }
    public BarrierTracker barrierTracker;
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

    public bool myTurn { get; set; }
    public bool isSummoned;

    public bool isDead;

    public RunData runData;

    [HideInInspector]public EntityUI myUI { get; set; }

    void Awake()
    {
        Initialize();
        TurnManager.deathPhase.AddListener(CheckForDeath);
    }

    public virtual void Initialize()
    {
        maxHealth = unitStats.maxHealth;
        damageScaling = unitStats.damageScaling;
        healScaling = unitStats.healScaling;
        barrierScaling = unitStats.barrierScaling;

        handSize = unitStats.handSize;
        turnSpeed = unitStats.turnSpeed;
        currentBeats = unitStats.startBeats;

        myTurn = false;
        TurnManager.initialPositionReport.AddListener(ReportCell);
        TurnManager.unitsReport.AddListener(RegisterTurn);

        myUI = GetComponentInChildren<EntityUI>();
    }


    public void RegisterTurn()
    {
        TurnManager.ReportTurn(this);
    }

    public void ReceiveDamage(int damage)
    {
        if (deflectHealth > 0) damage = barrierTracker.ReceiveDeflectDamage(damage);
        if (shieldHealth > 0) damage = barrierTracker.ReceiveShieldDamage(damage);

        ReduceHealth(damage);


        //update healthbar in UI
        myUI.UpdateHealth();
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
    }

    public void ReportCell()
    {
        //report our location to the cell we're in
        GameObject myTile = GridTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = this;
    }

    public void UnreportCell()
    {
        //report leaving a cell
        GameObject myTile = GridTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = null;
    }

    public virtual void Die()
    {
        if (gameObject.tag == "Enemy" && isSummoned != true)
        {
            //when an enemy dies, add its deck to the player's inventory for later use
            runData.essenceInventory.Add(GetComponent<Hand>().deckRecord);
        }
        TurnManager.UnreportTurn(this);
        UnreportCell();
        TurnManager.deathPhase.RemoveListener(CheckForDeath);
        isDead = true;
        VFXMachine.PlayAtLocation("Explosion", transform.position);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
