using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleEnder : MonoBehaviour
{
    [SerializeField] SceneRelay sceneRelay;
    [SerializeField] RunData runData;
    [SerializeField] SpawnPool bossPool;

    [SerializeField] GameObject winSign;
    [SerializeField] List<TMP_Text> winWords;

    [SerializeField] GameObject dropsSign;
    [SerializeField] TMP_Text dropAnnounce;
    [SerializeField] List<TMP_Text> dropsWords;

    [SerializeField] GameObject loseSign;
    [SerializeField] List<TMP_Text> loseWords;

    [SerializeField] Image blackScreen;

    public static List<Deck> deckDrops = new();

    /*private void Start()
    {
        deckDrops.AddRange(runData.essenceInventory);
        StartCoroutine(VictorySequence());
    }*/
    readonly int bossesForCredits = 3;
    bool returnToMap;
    public IEnumerator VictorySequence()
    {
        SoundManager.PlayMusic(SoundType.MUSICVICTORY);
        if (sceneRelay.bossEncounter)
        {
            Tutorial.CompleteStage(TutorialFor.WORLDBOSS, 1);
            runData.score += 1000;
        }
        else runData.score += 100;

        TurnManager.playerUnit.unitAnimator.Animate(AnimType.CHEER);
        winSign.SetActive(true);
        yield return StartCoroutine(FadeInWords(winWords));

        yield return StartCoroutine(FadeInDrops());

        yield return new WaitForSeconds(2f);

        int sceneTarget;
        if(runData.bossSequence.Count == bossPool.spawnUnits.Count - bossesForCredits && runData.endless == false) 
        {
            System.IO.File.Delete(Application.persistentDataPath + "/runData.json");
            UnlockNextDifficulty();
            sceneTarget = 3;
        }
        else sceneTarget = 1;
        yield return new WaitUntil(() => returnToMap == true);
        EventManager.loadSceneWithScreen.Invoke(sceneTarget);
        EventManager.loadSceneWithScreen.Invoke(-1);
    }

    public void ReturnToWorldMap() { returnToMap = true; }

    void UnlockNextDifficulty()
    {
        if (runData.difficultyTier == PlayerPrefs.GetInt("UnlockedDifficulty", 2))
            PlayerPrefs.SetInt("UnlockedDifficulty", PlayerPrefs.GetInt("UnlockedDifficulty", 2) + 1);
    }

    IEnumerator FadeInDrops()
    {
        List<string> drops = ProcessDropList(deckDrops);
        dropsWords.Insert(0, dropAnnounce);
        drops.Insert(0, "Essences Acquired:");
        dropsSign.SetActive(true);
        for (int i = 0; i < drops.Count; i++)
        {
            dropsWords[i].gameObject.SetActive(true);
            dropsWords[i].text = drops[i];
        }
        runData.essenceInventory.AddRange(deckDrops);
        deckDrops.Clear();
        yield return StartCoroutine(FadeInWords(dropsWords.Take(drops.Count).ToList(), .2f));
    }
    List<string> ProcessDropList(List<Deck> drops)
    {
        Dictionary<string, int> dropAmounts = new();

        foreach(var deck in drops)
        {
            deck.Initialize();
            if (!dropAmounts.ContainsKey(deck.unitName)) dropAmounts.Add(deck.unitName, 1);
            else dropAmounts[deck.unitName]++;
        }
        List<string> keys = dropAmounts.Keys.ToList();
        return keys.Select(x => $"* {x}: {dropAmounts[x]}x").ToList();
    }

    public IEnumerator DefeatSequence()
    {
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;
        System.IO.File.Delete(Application.persistentDataPath + "/runData.json");
        TurnManager.playerUnit.unitAnimator.Animate(AnimType.DIE);
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeScreenToBlack(blackScreen));
        loseSign.SetActive(true);
        yield return StartCoroutine(FadeInWords(loseWords));
    }

    public void ReturnToMain()
    {
        EventManager.loadSceneWithScreen.Invoke(0);
        EventManager.loadSceneWithScreen.Invoke(-1);
    }

    readonly Color32 invisWhite = new(255, 255, 255, 0);
    IEnumerator FadeInWords(List<TMP_Text> words, float duration = .3f)
    {
        foreach (var word in words)
        {
            word.gameObject.SetActive(true);
            word.color = invisWhite;
        }
        foreach (var word in words)
        {
            float timeElapsed = 0;
            while (timeElapsed < duration)
            {
                word.color = Color32.Lerp(invisWhite, Color.white, timeElapsed/duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    Color32 invisBlack = new(0, 0, 0, 0);
    IEnumerator FadeScreenToBlack(Image screen, float duration = 1f)
    {
        screen.gameObject.SetActive(true);

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            screen.color = Color32.Lerp(invisBlack, Color.black, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
