using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BuffTracker : MonoBehaviour
{
    public UnitActions stats;
    public BuffUI buffDisplay;

    GameObject myTile;
    List<TrackedBuff> buffs = new();

    class TrackedBuff
    {
        public CardEffectPlus lapseEffect;
        public CardEffectPlus endEffect;
        public int remainingDuration;
        public GameObject owner;
        public string[,] aoe;
    }
    void Awake()
    {
        TurnManager.turnDraw.AddListener(DurationProc);
    }

    public void RegisterBuff(GameObject ownerIn, EffectBuff buff, string[,] aoeIn)
    {
        myTile = GridTools.VectorToTile(gameObject.transform.position);
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
        if (gameObject.GetComponent<UnitActions>().myTurn)
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
                    buffs[i].endEffect.Execute(buffs[i].owner, myTile, buffs[i].aoe);
                    buffs.RemoveAt(i);
                }
            }
            buffDisplay.UpdateBuffs();
        }
    }
}


