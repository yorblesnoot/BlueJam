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
            if (remainingDuration <= 0)
            {
                token.gameObject.SetActive(false);
                return token;
            }
            return null;
        }

        public virtual IEnumerator TickEffect(BattleTileController tile) { yield break; }
    }

    class TrackedBuff : TrackedMod
    {
        public EffectRecurring buff;
        public override void InitialRender()
        {
            base.InitialRender();
            token.RenderBuff(buff, owner);
        }

        public override IEnumerator TickEffect(BattleTileController tile)
        {
            yield return tile.StartCoroutine(buff.turnLapseEffect.Execute(owner, tile));
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

        public override IEnumerator TickEffect(BattleTileController tile)
        {
            if (remainingDuration <= 0)
                stat.Unmodify(stat.scalingMultiplier, tile.OccupyingUnit());
            yield break;
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
        trackedStat.token = buffTokens[0];
        buffTokens.RemoveAt(0);
        trackedStat.InitialRender();
        mods.Add(trackedStat);
    }

    public IEnumerator DurationProc()
    {
        for (int i = 0; i < mods.Count; i++)
        {
            myTile = gameObject.OccupiedTile();
            BuffToken returning = mods[i].TickDown(myTile);
            yield return StartCoroutine(mods[i].TickEffect(myTile));
            if (returning != null) buffTokens.Add(returning);
        }
        mods.RemoveAll(x => x.remainingDuration == 0);
    }
}


