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
    [HideInInspector] public EntityUI myUI { get; set; }

    int hitLayer;
    int baseLayer;
    GameObject model;

    void Awake()
    {
        string modelName = gameObject.name.Replace("NPC(Clone)", "");
        model = transform.Find(modelName).gameObject;
        hitLayer = LayerMask.NameToLayer("HitFlash");
        baseLayer = LayerMask.NameToLayer("Default");
        Debug.Log(hitLayer);
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
        //abstract this out and add details~~~~~~~~~~~
        SetChildrenLayer(model, hitLayer);


        if (deflectHealth > 0) damage = barrierTracker.ReceiveDeflectDamage(damage);
        if (shieldHealth > 0) damage = barrierTracker.ReceiveShieldDamage(damage);

        ModifyHealth(damage);
        unitAnimator.Animate(AnimType.DAMAGED);
    }

    void SetChildrenLayer(GameObject target, int layer)
    {
        target.layer = layer;
        if (target.transform.childCount == 0) return;
        foreach(Transform child in target.transform)
        {
            SetChildrenLayer(child.gameObject, layer);
        }
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

    public void ReportCell()
    {
        BattleTileController myTile = MapTools.VectorToTile(gameObject.transform.position).GetComponent<BattleTileController>();
        myTile.unitContents = this;
        GetComponent<StencilControl>().ToggleStencil(myTile);
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

    public virtual void SpendBeats(int beats) { }

    public virtual void ShowInfoTag() { }
}
