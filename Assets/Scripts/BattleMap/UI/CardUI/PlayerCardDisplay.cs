using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardDisplay : MonoBehaviour, ICardDisplay
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] List<GameObject> costPips;
    [SerializeField] Image cardArt;
    [SerializeField] Image targetPane;
    [SerializeField] TMP_Text targetType;
    [SerializeField] TMP_Text effectText;
    [SerializeField] TMP_Text keywordPane;

    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    public EmphasizeCard emphasize;
    public GameObject cardBack;
    public bool forceConsume { get; set; } = false;

    //fill the details of a blank card
    public void PopulateCard(CardPlus card)
    {
        thisCard = card;
        forceConsume = thisCard.consumed;
        nameText.text = card.displayName;
        cardArt.sprite = card.art;
        foreach(GameObject pip in costPips) pip.SetActive(false);
        for(int pips = 0; pips < card.cost; pips++)
        {
            costPips[pips].SetActive(true);
        }
        effectText.text = card.description;
        if(card.cardClass.Contains(CardClass.MOVE) || card.cardClass.Contains(CardClass.SUMMON))
        {
            targetPane.color = new Color32(144, 144, 144, 255);
            targetType.text = "Target Empty";
        }
        else if(card.cardClass.Contains(CardClass.ATTACK))
        {
            if (card.aoePoint.GetLength(0) > 1) targetType.text = "AOE Enemy";
            else targetType.text = "Target Enemy";
            targetPane.color = new Color32(241, 124, 124, 255);
        }
        else if (card.cardClass.Contains(CardClass.BUFF))
        {
            if (card.aoePoint.GetLength(0) > 1) targetType.text = "AOE Ally";
            else targetType.text = "Target Ally";
            targetPane.color = new Color32(47, 231, 122, 255);
        }
        keywordPane.text = card.keywords;
    }
}
