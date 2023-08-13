using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalScoreDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text thisText;
    [SerializeField] RunData runData;
    void OnEnable()
    {
        float difficultyMod = 1 + runData.difficultyTier / 5;
        thisText.text = $"Your score was: {runData.score * difficultyMod}!";
    }
}
