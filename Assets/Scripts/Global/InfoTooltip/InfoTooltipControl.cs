using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoTooltipControl : MonoBehaviour
{
    [System.Serializable]
    class KeywordTooltip
    {
        public string linkId;
        public string text;
    }

    [SerializeField] List<KeywordTooltip> keywordTooltips;
    static Dictionary<string, string> keywordToTooltip = new();

    public static GameObject tooltip;
    public static TMP_Text tooltipText;

    [SerializeField] GameObject _tooltip;
    [SerializeField] TMP_Text _tooltipText;

    private void Awake()
    {
        keywordToTooltip = GenerateTooltipDictionary();
        tooltip = _tooltip;
        tooltipText = _tooltipText;
    }

    public Dictionary<string, string> GenerateTooltipDictionary()
    {
        Dictionary<string, string> dict = new();
        foreach (var keyword in keywordTooltips)
        {
            dict.Add(keyword.linkId, keyword.text);
        }
        return dict;
    }

    public static void ShowTooltip(string keywordId, TMP_Text textBox)
    {
        tooltip.SetActive(true);
        Vector3 tooltipPosition = textBox.transform.position;
        tooltipPosition.x += textBox.rectTransform.rect.width;
        tooltipPosition.x += tooltipText.rectTransform.rect.width;
        tooltip.transform.position = tooltipPosition;
        keywordId = keywordId.ToLower();
        if (keywordId.Contains("card_"))
        {
            //show a card preview
        }
        else
        {
            tooltipText.text = keywordToTooltip[keywordId];
        }
    }
    public static void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}
