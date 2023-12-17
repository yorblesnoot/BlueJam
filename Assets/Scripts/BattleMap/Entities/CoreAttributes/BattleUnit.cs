using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : Unit
{
    public BarrierTracker barrierTracker;
    public UnitStateFeedback stateFeedback;

    public BuffTracker buffTracker;
    public HandPlus myHand;
    [SerializeReference]
    public Animatable unitAnimator;

    public int deflectHealth { get; set; }
    public int shieldHealth { get; set; }

    public bool immovable;

    public Vector2Int mapPosition;

    [field:SerializeField] public bool isSummoned { get; set; }
    [HideInInspector] public bool isDead;
    [HideInInspector] public EntityUI myUI { get; set; }
    [HideInInspector] public CardPlus lastPlayed;

    [field: SerializeField] public AllegianceType Allegiance { get; set; }

    void Awake()
    {
        barrierTracker = new BarrierTracker(this);
        TurnManager.initializeDecks.AddListener(InitializeDecks);
        Initialize(); 
    }

    void InitializeDecks()
    {
        myHand.BuildVisualDeck();
        myHand.DrawPhase();
    }

    public virtual void TakeTurn()
    {
        barrierTracker.DeflectLapse();
    }

    public virtual void Initialize()
    {
        LoadStats();
        TurnManager.globalDeathCheck.AddListener(CheckForDeath);
        myUI = GetComponentInChildren<EntityUI>();
    }

    public void ReceiveDamage(int damage, GameObject source = null, bool animate = true)
    {
        if(animate) unitAnimator.Animate(AnimType.DAMAGED, source);
        if (deflectHealth > 0) damage = barrierTracker.ReceiveDeflectDamage(damage);
        if (shieldHealth > 0) damage = barrierTracker.ReceiveShieldDamage(damage);
        if (damage <= 0) return;

        stateFeedback.QueuePopup(damage, Color.red);
        StartCoroutine(stateFeedback.DamageFlash());
        
        ModifyHealth(damage);
    }

    

    public void CheckForDeath()
    {
        if (currentHealth <= 0) Die();
    }

    public virtual void ModifyHealth(int reduction)
    {
        currentHealth -= reduction;
        currentHealth = Mathf.Clamp(currentHealth, 0, Mathf.RoundToInt(loadedStats[StatType.MAXHEALTH]));
    }

    public virtual void Die()
    {
        unitAnimator.Animate(AnimType.DIE);
        SoundManager.PlaySound(SoundTypeEffect.DIE);
    }

    public virtual void SpendBeats(int beats) { }

    public virtual void ShowInfoTag() { }

    internal void HealHealth(int heal)
    {
        ModifyHealth(-heal);
        stateFeedback.QueuePopup(heal, Color.green);
    }
}
