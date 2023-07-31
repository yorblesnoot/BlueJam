using UnityEngine;
using UnityEngine.UI;

public class NPCCardDisplay : MonoBehaviour, ICardDisplay
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
    [SerializeField] ColoredSprite mixed;
    [SerializeField] ColoredSprite curse;

    //fill the details of a blank card
    public void PopulateCard(CardPlus card)
    {
        thisCard = card;
        forceConsume = thisCard.consumed;
        if (card.isCurse)
        {
            image.sprite = curse.sprite;
            image.color = curse.color;
        }
        else if (card.cardClass.Contains(CardClass.SUMMON))
        {
            image.sprite = summon.sprite;
            image.color = summon.color;
        }
        else if (card.cardClass.Contains(CardClass.MOVE))
        {
            if (card.cardClass.Contains(CardClass.ATTACK))
            {
                image.sprite = jumpAttack.sprite;
                image.color = jumpAttack.color;
                return;
            }
            image.sprite = move.sprite;
            image.color = move.color;
        }
        else if (card.cardClass.Contains(CardClass.ATTACK))
        {
            int range = card.targetRules.GetLength(0);
            foreach(CardEffectPlus effect in card.effects)
            {
                if((effect.aoe.GetLength(0) + range)/2 >= 2)
                {
                    image.sprite = rangeAttack.sprite;
                    image.color = rangeAttack.color;
                    return;
                }
            }
            image.sprite = meleeAttack.sprite;
            image.color = meleeAttack.color;
        }
        else if (card.cardClass.Contains(CardClass.BUFF))
        {
            image.sprite = heal.sprite;
            image.color = heal.color;
        }

    }
}
