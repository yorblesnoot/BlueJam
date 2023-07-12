using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTracker : MonoBehaviour
{
    public BattleUnit stats;
    public BuffUI buffDisplay;

    BattleTileController myTile;
    List<TrackedBuff> buffs = new();

    class TrackedBuff
    {
        public CardEffectPlus lapseEffect;
        public CardEffectPlus endEffect;
        public int remainingDuration;
        public BattleUnit owner;
        public bool[,] aoe;
    }

    public void RegisterBuff(BattleUnit ownerIn, EffectBuff buff)
    {
        TrackedBuff incomingBuff = new() {
            lapseEffect = buff.turnLapseEffect,
            endEffect = buff.removalEffect,
            remainingDuration = buff.duration,
            owner = ownerIn};
        incomingBuff.endEffect?.Initialize();
        incomingBuff.lapseEffect?.Initialize();
        buffs.Add(incomingBuff);
        buffDisplay.DisplayBuff(buff);
    }

    public void DurationProc()
    {
        myTile = MapTools.VectorToTile(gameObject.transform.position).GetComponent<BattleTileController>();
        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].remainingDuration--;
            if (buffs[i].lapseEffect != null)
                StartCoroutine(buffs[i].lapseEffect.Execute(buffs[i].owner, myTile));
            if (buffs[i].remainingDuration <= 0)
            {
                //remove the buff in question
                if (buffs[i].endEffect != null)
                    StartCoroutine(buffs[i].endEffect.Execute(buffs[i].owner, myTile)); 
                buffs.RemoveAt(i);
            }
        }
        buffDisplay.TickDownBuffTokens();
    }
}


