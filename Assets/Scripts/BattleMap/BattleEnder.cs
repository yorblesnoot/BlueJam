using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEnder : MonoBehaviour
{
    public RunData runData;
    public void EndCombat()
    {
        //send the player back to the world map
        StartCoroutine(WaitToEnd());
    }

    private IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
    }
}
