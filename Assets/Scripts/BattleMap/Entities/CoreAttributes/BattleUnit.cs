using UnityEngine;

public class BattleUnit : MonoBehaviour, IPlayerStats
{
    [field: SerializeField] public UnitStats unitStats { get; set; }
    [SerializeField] BarrierTracker barrierTracker;
    [SerializeField] BuffTracker buffTracker;
    public HandPlus myHand;
    public UnitAnimator unitAnimator;

    public int maxHealth { get; set; }

    private int handSize; 
    public int HandSize
    {
        get => handSize; 
        set { handSize = Mathf.Clamp(value, 1, 7); }
    }
    private float turnSpeed;
    public float TurnSpeed
    {
        get => turnSpeed; 
        set { turnSpeed = Mathf.Clamp(value, .2f, value); }
    }

    public int currentHealth { get; set; }
    public int deflectHealth { get; set; }
    public int shieldHealth { get; set; }

    public float currentBeats { get; set; }

    private int damageScaling;
    public int DamageScaling
    {
        get => damageScaling;
        set { damageScaling = Mathf.Clamp(value, 5, value); }
    }
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
        //buff lapse effects
        TurnTick();
        TurnManager.deathPhase?.Invoke();
        if (isDead)
        {
            TurnManager.AssignTurn();
            return;
        }
        GetAction();
    }

    void TurnTick()
    {
        buffTracker.DurationProc();
        barrierTracker.DeflectLapse();
    }

    public virtual void GetAction() { }

    public virtual void Initialize()
    {
        maxHealth = unitStats.maxHealth;
        DamageScaling = unitStats.damageScaling;
        healScaling = unitStats.healScaling;
        barrierScaling = unitStats.barrierScaling;
        HandSize = unitStats.handSize;
        TurnSpeed = unitStats.turnSpeed;
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
