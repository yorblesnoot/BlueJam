using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    public Slider sliderHealth;
    public Slider sliderBeats;
    public Slider sliderDeflect;
    public Slider sliderShield;

    public GameObject blankCard;
    public Hand myHand;
    public GameObject unitCanvas;
    public UnitActions unitActions;

    [field: SerializeField]public BuffUI buffUI { get; set; }

    public int cardSize;
    public void UpdateHealth()
    {
        StartCoroutine(UpdateBar(unitActions.currentHealth, unitActions.maxHealth, sliderHealth));
        if (unitActions.deflectHealth > 0) sliderDeflect.gameObject.SetActive(true);
        if (unitActions.shieldHealth > 0) sliderShield.gameObject.SetActive(true);

        StartCoroutine(UpdateBar(unitActions.deflectHealth, unitActions.maxHealth, sliderDeflect));
        StartCoroutine(UpdateBar(unitActions.shieldHealth, unitActions.maxHealth, sliderShield));
    }

    public virtual void UpdateBeats()
    {
        StartCoroutine(UpdateBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderBeats));
    }

    public virtual IEnumerator UpdateBar(float current, float max, Slider slider)
    {
        float changeInterval = 10;

        float toValue = current / max;

        float mod = (slider.value - toValue) / changeInterval;
        for (int count = 0; count < changeInterval; count++)
        {
            slider.value -= mod;
            yield return new WaitForSeconds(.01f);
        }
        if(slider.value == 0 && slider != sliderBeats)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public GameObject RenderBlank(CardPlus card)
    {
        //scale and rotation for cards 
        Quaternion rotate = Quaternion.Euler(0, 0, 0);

        GameObject newCard = Instantiate(blankCard, new Vector3(0, -20, 0), rotate);
        newCard.transform.SetParent(unitCanvas.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        ICardDisplay cardDisplay = newCard.GetComponent<ICardDisplay>();
        cardDisplay.owner = gameObject;
        cardDisplay.PopulateCard(card);

        return newCard;
    }

    public virtual void PositionCards() { }
}