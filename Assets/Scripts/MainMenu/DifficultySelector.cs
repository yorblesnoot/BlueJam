using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultySelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    class DifficultySetting
    {
        public string difficultyName;
        public BalanceSettings difficultyParams;
        public string tooltip;
        public Color color;
    }

    [SerializeField] TMP_Text difficultyDisplay;
    [SerializeField] GameObject tooltipContainer;
    [SerializeField] TMP_Text tooltipText;

    [SerializeField] GameObject arrowEasier;
    [SerializeField] GameObject arrowHarder;

    [SerializeField] List<DifficultySetting> difficulties;
    public int currentDifficulty;

    private void Awake()
    {
        currentDifficulty = PlayerPrefs.GetInt("UnlockedDifficulty", 2);
        if (currentDifficulty == difficulties.Count) currentDifficulty--;
        UpdateDifficultyDisplay(currentDifficulty);
    }

    public void IncrementDifficulty()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        currentDifficulty++;
        UpdateDifficultyDisplay(currentDifficulty);
    }

    public void DecrementDifficulty()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        currentDifficulty--;
        UpdateDifficultyDisplay(currentDifficulty);
    }
    
    void UpdateDifficultyDisplay(int level)
    {
        difficultyDisplay.text = difficulties[level].difficultyName;
        tooltipText.text = difficulties[currentDifficulty].tooltip;
        difficultyDisplay.color = difficulties[level].color;
        if (currentDifficulty == difficulties.Count - 1 || currentDifficulty == PlayerPrefs.GetInt("UnlockedDifficulty", 2)) arrowHarder.SetActive(false);
        else arrowHarder.SetActive(true);
        if (currentDifficulty == 0) arrowEasier.SetActive(false);
        else arrowEasier.SetActive(true);
    }
    public BalanceSettings GetDifficultyFromTier(int tier)
    {
        return difficulties[tier].difficultyParams;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(PrepareTooltip());
    }

    IEnumerator PrepareTooltip()
    {
        yield return new WaitForSeconds(1);
        tooltipContainer.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipContainer.SetActive(false);
        StopAllCoroutines();
    }
}


