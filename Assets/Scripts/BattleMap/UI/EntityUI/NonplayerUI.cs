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
        pips.SetPips(Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]));
    }

    public void UpdateBeats()
    {
        if (!unitActions.isDead)
        {
            SetBar(unitActions.loadedStats[StatType.BEATS], TurnManager.beatThreshold + 2, sliderGhostBeats, false);
            StartCoroutine(UpdateBar(unitActions.loadedStats[StatType.BEATS], TurnManager.beatThreshold + 2, sliderBeats, false));
        }
        else TurnManager.updateBeatCounts.RemoveListener(UpdateBeats);
    }

    public void ShowBeatGhost(float beats)
    {
        SetBar(unitActions.loadedStats[StatType.BEATS] + beats, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
    }
    public void HideBeatGhost()
    {
        SetBar(unitActions.loadedStats[StatType.BEATS], TurnManager.beatThreshold + 2, sliderGhostBeats, false);
    }
}
