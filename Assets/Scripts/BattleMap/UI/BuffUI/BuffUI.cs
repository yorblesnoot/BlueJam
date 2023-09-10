using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    public List<BuffToken> buffTokens;

    class TimedBuff
    {
        public BuffToken token;
        public int remainingDuration;
    }

    List<TimedBuff> timedBuffs = new();

    public void DisplayBuff(int duration, Color32 iconColor, string description)
    {
        int buffLocation = timedBuffs.Count;
        if(buffLocation >= buffTokens.Count)
        {
            var newToken = Instantiate(buffTokens[0].gameObject);
            newToken.transform.SetParent(buffTokens[0].transform.parent, false);
            buffTokens.Add(newToken.GetComponent<BuffToken>());
        }
        TimedBuff timedBuff = new() { token = buffTokens[buffLocation], remainingDuration = duration };
        buffTokens[buffLocation].gameObject.SetActive(true);
        buffTokens[buffLocation].RenderBuff(iconColor, duration, description);
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

