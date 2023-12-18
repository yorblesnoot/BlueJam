using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCCardDisplay : MonoBehaviour, ICardDisplay //, IPointerEnterHandler, IPointerExitHandler
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    public bool forceConsume { get; set; } = false;

    [System.Serializable]
    class ColoredSprite
    {
        public Sprite sprite;
        public Color color;
    }

    [SerializeField] Image image;

    [SerializeField] ColoredSprite meleeAttack;
    [SerializeField] ColoredSprite rangeAttack;
    [SerializeField] ColoredSprite jumpAttack;
    [SerializeField] ColoredSprite heal;
    [SerializeField] ColoredSprite summon;
    [SerializeField] ColoredSprite move;
    [SerializeField] ColoredSprite curse;
    [SerializeField] ColoredSprite corrupt;

    //fill the details of a blank card
    public void PopulateCard(CardPlus card, Unit owner, bool limited = false)
    {
        List<CardClass> cardClass = card.effects.Select(x => x.effectClass).ToList();
        thisCard = card;
        forceConsume = thisCard.consumed;
        if(card.cardType == CardType.CORRUPTION)
        {
            image.sprite = corrupt.sprite;
            image.color = corrupt.color;
        }
        else if (card.cardType == CardType.CURSE)
        {
            image.sprite = curse.sprite;
            image.color = curse.color;
        }
        else if (cardClass.Contains(CardClass.SUMMON))
        {
            image.sprite = summon.sprite;
            image.color = summon.color;
        }
        else if (cardClass.Contains(CardClass.MOVE))
        {
            if (cardClass.Contains(CardClass.ATTACK))
            {
                image.sprite = jumpAttack.sprite;
                image.color = jumpAttack.color;
                return;
            }
            image.sprite = move.sprite;
            image.color = move.color;
        }
        else if (cardClass.Contains(CardClass.ATTACK))
        {
            int range = card.targetRules.GetLength(0);
            foreach(CardEffectPlus effect in card.effects)
            {
                if((effect.aoe.GetLength(0) + range)/2 >= 3)
                {
                    image.sprite = rangeAttack.sprite;
                    image.color = rangeAttack.color;
                    return;
                }
            }
            image.sprite = meleeAttack.sprite;
            image.color = meleeAttack.color;
        }
        else if (cardClass.Contains(CardClass.BUFF))
        {
            image.sprite = heal.sprite;
            image.color = heal.color;
        }
    }

    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(CountToProject());
    }

    float projectDelay = .5f;
    IEnumerator CountToProject()
    {
        yield return new WaitForSeconds(projectDelay);
        CardProjector.ProjectCard(thisCard, owner, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        CardProjector.HideProjectedCard();
    }
    */
}
