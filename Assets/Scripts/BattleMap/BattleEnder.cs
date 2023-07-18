using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleEnder : MonoBehaviour
{
    public SceneRelay sceneRelay;
    [SerializeField] RunData runData;
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
    public IEnumerator VictorySequence()
    {
        if (sceneRelay.bossEncounter) runData.score += 1000;
        else runData.score += 100;

        TurnManager.playerUnit.unitAnimator.Animate(AnimType.CHEER);
        winSign.SetActive(true);
        yield return StartCoroutine(FadeInWords(winWords));

        yield return StartCoroutine(FadeInDrops());

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
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
        yield return StartCoroutine(FadeInWords(dropsWords.Take(drops.Count).ToList(), 17));
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
        SceneManager.LoadScene(0);
    }

    IEnumerator FadeInWords(List<TMP_Text> words, int interval = 5)
    {
        foreach (var word in words)
        {
            word.color = new Color32(255, 255, 255, 0);
        }
        foreach (var word in words)
        {
            Color32 colorCurrent = new(255, 255, 255, 0);
            word.gameObject.SetActive(true);
            for (int i = 0; i < 255; i += interval)
            {
                colorCurrent.a = (byte)i;
                word.color = colorCurrent;
                yield return new WaitForSeconds(.02f);
            }
        }
    }

    IEnumerator FadeScreenToBlack(Image screen)
    {
        screen.gameObject.SetActive(true);
        Color32 colorCurrent = new(0, 0, 0, 0);
        for (int i = 0; i < 255; i += 5)
        {
            colorCurrent.a = (byte)i;
            screen.color = colorCurrent;
            yield return new WaitForSeconds(.02f);
        }
    }
}
