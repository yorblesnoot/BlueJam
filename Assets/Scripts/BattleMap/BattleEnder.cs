using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEnder : MonoBehaviour
{
    public SceneRelay sceneRelay;
    [SerializeField] GameObject winSign;
    [SerializeField] List<TMP_Text> words;

    public void EndCombat()
    {
        //send the player back to the world map
        StartCoroutine(WaitToEnd());
    }

    private IEnumerator WaitToEnd()
    {
        TurnManager.playerUnit.unitAnimator.Animate(AnimType.CHEER);
        winSign.SetActive(true);
        foreach (var word in words)
        {
            word.color = new Color32(255, 255, 255, 0);
        }
        foreach (var word in words)
        {
            Color32 colorCurrent = new Color32(255, 255, 255, 0);
            for (int i = 0; i < 255; i+=5)
            {
                colorCurrent.a = (byte)i;
                word.color = colorCurrent;
                yield return new WaitForSeconds(.02f);
            }
        }
        
        yield return new WaitForSeconds(2f);
        if (sceneRelay.bossEncounter == true)
        {
            //send the player to credits if they beat the boss
            System.IO.File.Delete(Application.persistentDataPath + "/runData.json");
            SceneManager.LoadScene(3);
        }
        else SceneManager.LoadScene(1);
    }
}
