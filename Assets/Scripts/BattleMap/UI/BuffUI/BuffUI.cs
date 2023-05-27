using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffUI : MonoBehaviour
{
    public List<GameObject> buffSlots;
    List<BuffToken> buffTokens;

    List<EffectBuff> buffs;
    private void Awake()
    {
        buffs = new List<EffectBuff>();
        buffTokens = new List<BuffToken>();
        for(int i = 0; i < buffSlots.Count; i++)
        {
            buffTokens.Add(buffSlots[i].GetComponent<BuffToken>());
            buffSlots[i].SetActive(false);
        }
    }

    public void DisplayBuff(EffectBuff newBuff)
    {
        buffs.Add(newBuff);
        int buffLocation = buffs.IndexOf(newBuff);
        buffSlots[buffLocation].SetActive(true);
        buffTokens[buffLocation].RenderBuff(newBuff.iconColor,newBuff.duration);
    }

    public void UpdateBuffs()
    {
        for (int i = 0; i < buffSlots.Count; i++)
        {
            if (buffSlots[i].activeSelf)
            {
                buffTokens[i].DecrementBuff();
            }
        }
    }

    public void HideBuff(EffectBuff buff)
    {
        int buffLocation = buffs.IndexOf(buff);
        buffs.RemoveAt(buffLocation);
        buffSlots[buffLocation].SetActive(false);
    }
}

