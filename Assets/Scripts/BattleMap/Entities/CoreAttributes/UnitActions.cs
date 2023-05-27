using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActions : MonoBehaviour, IPlayerData
{
    [field: SerializeField] public UnitStats unitStats { get; set; }
    public BarrierTracker barrierTracker;

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

    [HideInInspector]public EntityUI myUI { get; set; }

    void Awake()
    {
        Initialize(); 
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
        TurnManager.finishUpdate.AddListener(ReportCell);
        TurnManager.unitsReport.AddListener(RegisterTurn);

        myUI = GetComponentInChildren<EntityUI>();
    }


    public void RegisterTurn()
    {
        myUI.UpdateHealth();
        TurnManager.ReportTurn(gameObject);
    }

    public void ReceiveDamage(int damage)
    {
        if (deflectHealth > 0) damage = barrierTracker.ReceiveDeflectDamage(damage);
        if (shieldHealth > 0) damage = barrierTracker.ReceiveShieldDamage(damage);

        ReduceHealth(damage);

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        //update healthbar in UI
        myUI.UpdateHealth();

        if (currentHealth <= 0) Die();
    }

    public virtual void ReduceHealth(int reduction)
    {
        currentHealth -= reduction;
    }

    public void ReportCell()
    {
        //report our location to the cell we're in
        GameObject myTile = GridTools.VectorToTile(gameObject.transform.position);
        myTile.GetComponent<BattleTileController>().unitContents = gameObject;
    }

    public virtual void Die()
    {
        if (gameObject.tag == "Enemy" && isSummoned != true)
        {
            //when an enemy dies, add its deck to the player's inventory for later use
            BattleEnder battleEnder = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleEnder>();
            battleEnder.runData.essenceInventory.Add(GetComponent<Hand>().deckRecord);
        }
        TurnManager.UnreportTurn(gameObject);
        isDead = true;
        StartCoroutine(DieSlowly());
    }

    IEnumerator DieSlowly()
    {
        yield return new WaitForSeconds(.3f);
        VFXMachine.PlayAtLocation("Explosion", transform.position);
        gameObject.SetActive(false);
    }
}
