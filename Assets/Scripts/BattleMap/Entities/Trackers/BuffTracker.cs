using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTracker : MonoBehaviour
{
    public List<BuffToken> buffTokens;

    BattleTileController myTile;
    List<TrackedMod> mods = new();

    class TrackedMod
    {
        public int remainingDuration;
        public BattleUnit owner;
        public BuffToken token;

        public virtual void InitialRender()
        {
            token.gameObject.SetActive(true);
        }

        public BuffToken TickDown(BattleTileController tile)
        {
            remainingDuration--;
            token.SetDuration(remainingDuration);
            TickEffect(tile);
            if (remainingDuration <= 0)
            {
                token.gameObject.SetActive(false);
                return token;
            }
            return null;
        }

        public virtual void TickEffect(BattleTileController tile) { }
    }

    class TrackedBuff : TrackedMod
    {
        public EffectRecurring buff;
        public override void InitialRender()
        {
            base.InitialRender();
            token.RenderBuff(buff, owner);
        }

        public override void TickEffect(BattleTileController tile)
        {
            tile.unitContents.StartCoroutine(buff.turnLapseEffect.Execute(owner, tile));
        }
    }

    class TrackedStat: TrackedMod
    {
        public EffectStat stat;
        public override void InitialRender()
        {
            base.InitialRender();
            token.RenderStat(stat, owner);
        }

        public override void TickEffect(BattleTileController tile)
        {
            if (remainingDuration <= 0)
                stat.Unmodify(stat.scalingMultiplier, tile.unitContents);
        }
    }

    public void AddTokenIfNeeded()
    {
        if (mods.Count >= buffTokens.Count)
        {
            var newToken = Instantiate(buffTokens[0].gameObject);
            newToken.transform.SetParent(buffTokens[0].transform.parent, false);
            buffTokens.Add(newToken.GetComponent<BuffToken>());
        }
    }

    public void RegisterRecurring(BattleUnit ownerIn, EffectRecurring buff)
    {
        AddTokenIfNeeded();
        TrackedBuff incomingBuff = new() {
            buff = buff,
            remainingDuration = buff.duration,
            owner = ownerIn};
        buff.turnLapseEffect.Initialize(); 
        incomingBuff.token = buffTokens[0];
        buffTokens.RemoveAt(0);
        incomingBuff.InitialRender();
        mods.Add(incomingBuff);
    }

    public void RegisterTempStat(EffectStat stat, BattleUnit owner)
    {
        var trackedStat = new TrackedStat
        {
            stat = stat,
            remainingDuration = stat.duration,
            owner = owner
        };
        trackedStat.InitialRender();
        mods.Add(trackedStat);
    }

    public void DurationProc()
    {
        myTile = MapTools.VectorToTile(gameObject.transform.position).GetComponent<BattleTileController>();
        for (int i = 0; i < mods.Count; i++)
        {
            BuffToken returning = mods[i].TickDown(myTile);
            if(returning != null) buffTokens.Add(returning);
        }
        mods.RemoveAll(x => x.remainingDuration == 0);
    }
}


