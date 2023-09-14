using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;


public class NonplayerUI : EntityUI
{
    public Slider sliderBeats;
    public Slider sliderGhostBeats;
    [SerializeField] GameObject beatPip;
    [SerializeField] NPCHealthPips pips;

    readonly int beatBarSpace = 2;
    Coroutine beatUpdater;
    
    void Awake()
    {
        unitActions = GetComponentInParent<BattleUnit>();
        EventManager.hideTurnDisplay.AddListener(HideBeatGhost);
    } 

    public override void InitializeHealth()
    {
        base.InitializeHealth();
        pips.SetPips(Mathf.RoundToInt(unitActions.loadedStats[StatType.MAXHEALTH]));
        sliderBeats.value = unitActions.loadedStats[StatType.BEATS];
        SetBeatMax();
    }

    public override void UpdateDeflect()
    {
        base.UpdateDeflect();
        sliderDeflect.gameObject.SetActive(sliderDeflect.value > 1);
    }

    public override void UpdateShield()
    {
        base.UpdateShield();
        sliderShield.gameObject.SetActive(sliderShield.value > 1);
    }

    public override void ReduceBeats(float beatChange)
    {
        if (unitActions.isDead) return;
        HideBeatGhost();
        SetBeatMax(beatChange);
        if(beatUpdater != null) StopCoroutine(beatUpdater);
        beatUpdater = StartCoroutine(UpdateBeatBar());
    }

    readonly float beatTime = .3f;
    public IEnumerator UpdateBeatBar()
    {
        float timeElapsed = 0;
        float start = sliderBeats.value;
        while (timeElapsed < beatTime)
        {
            sliderBeats.value = Mathf.Lerp(start, unitActions.loadedStats[StatType.BEATS], timeElapsed / beatTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        sliderBeats.value = unitActions.loadedStats[StatType.BEATS];
        sliderGhostBeats.value = sliderBeats.value;
    }

    void SetBeatMax(float beatChange = 0)
    {
        float newMax = Mathf.Max(TurnManager.beatThreshold + beatBarSpace, sliderBeats.value - beatChange);
        sliderBeats.maxValue = newMax;
        sliderGhostBeats.maxValue = newMax;

        int segmentAmount = Mathf.RoundToInt(sliderBeats.maxValue - sliderBeats.minValue);
        float barWidth = sliderBeats.GetComponent<RectTransform>().rect.width;
        float segmentSize = barWidth/segmentAmount;
        int segmentCount = TurnManager.beatThreshold - Mathf.RoundToInt(sliderBeats.minValue);
        float pipX = barWidth/-2 + (segmentSize * segmentCount);
        Vector3 finalPosition = beatPip.transform.localPosition;
        finalPosition.x = pipX;
        StartCoroutine(beatPip.LerpTo(finalPosition, .2f, true));
        //beatPip.transform.localPosition = finalPosition; 
    }

    public void ShowBeatGhost(float beats)
    {
        sliderGhostBeats.value = unitActions.loadedStats[StatType.BEATS] + beats;
    }
    public void HideBeatGhost()
    {
        sliderGhostBeats.value = unitActions.loadedStats[StatType.BEATS];
    }
}
