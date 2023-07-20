using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NonplayerUI : EntityUI
{
    public Slider sliderBeats;
    public Slider sliderGhostBeats;
    [SerializeField] NPCHealthPips pips;
    

    //contents of hand
    
    void Awake()
    {
        unitActions = GetComponentInParent<BattleUnit>();
        TurnManager.updateBeatCounts.AddListener(UpdateBeats);
        EventManager.hideTurnDisplay.AddListener(HideBeatGhost);
    } 

    public override void InitializeHealth()
    {
        base.InitializeHealth();
        pips.SetPips(unitActions.maxHealth);
    }

    public void UpdateBeats()
    {
        if (!unitActions.isDead)
        {
            SetBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
            StartCoroutine(UpdateBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderBeats, false));
        }
        else TurnManager.updateBeatCounts.RemoveListener(UpdateBeats);
    }

    public void ShowBeatGhost(float beats)
    {
        SetBar(unitActions.currentBeats + beats, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
    }
    public void HideBeatGhost()
    {
        SetBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
    }
}
