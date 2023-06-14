using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NPCCardDisplay : MonoBehaviour, ICardDisplay
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    public Image image;

    public Sprite attack;
    public Sprite jumpAttack;
    public Sprite heal;
    public Sprite summon;
    public Sprite move;
    public Sprite mixed;

    //fill the details of a blank card
    public void PopulateCard(CardPlus card)
    {
        if (card.cardClass.Contains(CardClass.SUMMON))
        {
            image.sprite = summon;
            image.color = new Color32(176, 176, 176, 255); //gray

        }
        else if (card.cardClass.Contains(CardClass.ATTACK) && card.cardClass.Contains(CardClass.BUFF))
        {
            image.sprite = mixed;
            image.color = new Color32(253, 223, 30, 255); //yellow

        }
        else if (card.cardClass.Contains(CardClass.MOVE) && card.cardClass.Contains(CardClass.ATTACK))
        {
            image.sprite = jumpAttack;
            image.color = new Color32(181, 77, 240, 255); //purple
        }
        else if (card.cardClass.Contains(CardClass.MOVE))
        {
            image.sprite = move;
            image.color = new Color32(77, 161, 246, 255); //blue
        }
        else if (card.cardClass.Contains(CardClass.ATTACK))
        {
            image.sprite = attack;
            image.color = new Color32(249, 61, 40, 255); //red
        }
        else if (card.cardClass.Contains(CardClass.BUFF))
        {
            image.sprite = heal;
            image.color = new Color32(73, 240, 90, 255); //green
        }

    }
}
