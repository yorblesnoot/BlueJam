using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    public List<BuffToken> buffTokens;

    class TimedBuff
    {
        public EffectBuff buff;
        public BuffToken token;
        public int remainingDuration;
    }

    List<TimedBuff> timedBuffs = new();

    public void DisplayBuff(EffectBuff newBuff)
    {
        int buffLocation = timedBuffs.Count;
        TimedBuff timedBuff = new() { buff = newBuff, token = buffTokens[buffLocation], remainingDuration = newBuff.duration };
        buffTokens[buffLocation].gameObject.SetActive(true);
        buffTokens[buffLocation].RenderBuff(newBuff.iconColor,newBuff.duration);
        timedBuffs.Add(timedBuff);
    }

    public void TickDownBuffTokens()
    {
        for (int i = 0; i < timedBuffs.Count; i++)
        {
            timedBuffs[i].remainingDuration--;
            if (timedBuffs[i].remainingDuration <= 0)
            {
                timedBuffs[i].token.gameObject.SetActive(false);
                timedBuffs.RemoveAt(i);
            }
            else timedBuffs[i].token.SetDuration(timedBuffs[i].remainingDuration);
        }
    }
}

