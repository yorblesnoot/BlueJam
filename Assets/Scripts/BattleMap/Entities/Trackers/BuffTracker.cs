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
        public string[,] aoe;
    }
    void Awake()
    {
        TurnManager.drawThenBuffPhase.AddListener(DurationProc);
    }

    public void RegisterBuff(BattleUnit ownerIn, EffectBuff buff, string[,] aoeIn)
    {
        myTile = GridTools.VectorToTile(gameObject.transform.position).GetComponent<BattleTileController>();
        TrackedBuff incomingBuff = new TrackedBuff {
            lapseEffect = buff.turnLapseEffect,
            endEffect = buff.removalEffect,
            remainingDuration = buff.duration,
            owner = ownerIn,
            aoe = aoeIn};
        buffs.Add(incomingBuff);
        buffDisplay.DisplayBuff(buff);
    }

    void DurationProc()
    {
        if (gameObject.GetComponent<BattleUnit>().myTurn)
        {
            for (int i = 0; i < buffs.Count; i++)
            {
                buffs[i].remainingDuration--;
                if (buffs[i] != null)
                {
                    buffs[i].lapseEffect.Execute(buffs[i].owner, myTile, buffs[i].aoe);
                }
                if (buffs[i].remainingDuration <= 0)
                {
                    //remove the buff in question
                    try { buffs[i].endEffect.Execute(buffs[i].owner, myTile, buffs[i].aoe); }
                    catch { }
                    buffs.RemoveAt(i);
                }
            }
            buffDisplay.UpdateBuffs();
        }
    }
}

