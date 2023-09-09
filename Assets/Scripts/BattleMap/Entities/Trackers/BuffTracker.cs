using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTracker : MonoBehaviour
{
    public BuffUI buffDisplay;

    BattleTileController myTile;
    List<TrackedBuff> buffs = new();
    List<TrackedStat> statMods = new();

    class TrackedBuff
    {
        public CardEffectPlus lapseEffect;
        public int remainingDuration;
        public BattleUnit owner;
        public bool[,] aoe;
    }

    class TrackedStat
    {
        public EffectStat effectStat;
        public int remainingDuration;
    }

    public void RegisterRecurring(BattleUnit ownerIn, EffectRecurring buff)
    {
        TrackedBuff incomingBuff = new() {
            lapseEffect = buff.turnLapseEffect,
            remainingDuration = buff.duration,
            owner = ownerIn};
        incomingBuff.lapseEffect.Initialize();
        buffs.Add(incomingBuff);
        buffDisplay.DisplayBuff(buff.duration, buff.iconColor, buff.turnLapseEffect.GenerateDescription(ownerIn).FirstToUpper() +" after each action.");
    }

    public void RegisterTempStat(EffectStat stat)
    {
        statMods.Add(new TrackedStat
        {
            effectStat = stat,
            remainingDuration = stat.duration
        });
        buffDisplay.DisplayBuff(stat.duration, Color.yellow, stat.GenerateDescription(GetComponent<Unit>()).FirstToUpper() + ".");
    }

    public void DurationProc()
    {
        myTile = MapTools.VectorToTile(gameObject.transform.position).GetComponent<BattleTileController>();
        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].remainingDuration--;
            StartCoroutine(buffs[i].lapseEffect.Execute(buffs[i].owner, myTile));
        }
        buffs.RemoveAll(x => x.remainingDuration == 0);
        buffDisplay.TickDownBuffTokens();

        for (int i = 0; i < statMods.Count; i++)
        {
            statMods[i].remainingDuration--;
            if (statMods[i].remainingDuration == 0)
            {
                EffectStat stat = statMods[i].effectStat;
                stat.Unmodify(stat.scalingMultiplier, myTile.unitContents);
            }
        }
        statMods.RemoveAll(x => x.remainingDuration == 0);
    }
}


