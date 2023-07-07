using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleEnder : MonoBehaviour
{
    public SceneRelay sceneRelay;
    [SerializeField] GameObject winSign;
    [SerializeField] List<TMP_Text> winWords;

    [SerializeField] GameObject loseSign;
    [SerializeField] List<TMP_Text> loseWords;

    [SerializeField] Image blackScreen;

    public IEnumerator VictorySequence()
    {
        TurnManager.playerUnit.unitAnimator.Animate(AnimType.CHEER);
        yield return StartCoroutine(FadeInWords(winSign, winWords));

        yield return new WaitForSeconds(2f);
        if (sceneRelay.bossEncounter == true)
        {
            //send the player to credits if they beat the boss
            System.IO.File.Delete(Application.persistentDataPath + "/runData.json");
            SceneManager.LoadScene(3);
        }
        else SceneManager.LoadScene(1);
    }

    public IEnumerator DefeatSequence()
    {
        PlayerUnit.playerState = PlayerBattleState.AWAITING_TURN;
        System.IO.File.Delete(Application.persistentDataPath + "/runData.json");
        TurnManager.playerUnit.unitAnimator.Animate(AnimType.DIE);
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeScreenToBlack(blackScreen));
        yield return StartCoroutine(FadeInWords(loseSign, loseWords));
        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene(0);
    }

    IEnumerator FadeInWords(GameObject sign, List<TMP_Text> words)
    {
        sign.SetActive(true);
        foreach (var word in words)
        {
            word.color = new Color32(255, 255, 255, 0);
        }
        foreach (var word in words)
        {
            Color32 colorCurrent = new(255, 255, 255, 0);
            for (int i = 0; i < 255; i += 5)
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
