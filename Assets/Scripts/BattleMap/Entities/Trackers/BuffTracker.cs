using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    void Awake()
    {
        TurnManager.drawThenBuffPhase.AddListener(DurationProc);
    }

    public void RegisterBuff(BattleUnit ownerIn, EffectBuff buff, bool[,] aoeIn)
    {
        TrackedBuff incomingBuff = new() {
            lapseEffect = buff.turnLapseEffect,
            endEffect = buff.removalEffect,
            remainingDuration = buff.duration,
            owner = ownerIn,
            aoe = aoeIn};
        incomingBuff.endEffect?.Initialize();
        incomingBuff.lapseEffect?.Initialize();
        buffs.Add(incomingBuff);
        buffDisplay.DisplayBuff(buff);
    }

    void DurationProc()
    {
        if (gameObject.GetComponent<BattleUnit>().myTurn)
        {
            myTile = MapTools.VectorToTile(gameObject.transform.position).GetComponent<BattleTileController>();
            for (int i = 0; i < buffs.Count; i++)
            {
                buffs[i].remainingDuration--;
                buffs[i]?.lapseEffect.Execute(buffs[i].owner, myTile);
                if (buffs[i].remainingDuration <= 0)
                {
                    //remove the buff in question
                    try { buffs[i].endEffect.Execute(buffs[i].owner, myTile); }
                    catch { }
                    buffs.RemoveAt(i);
                }
            }
            buffDisplay.TickDownBuffTokens();
        }
    }
}


