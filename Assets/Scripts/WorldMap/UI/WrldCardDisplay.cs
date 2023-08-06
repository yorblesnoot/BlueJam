using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WrldCardDisplay : PlayerCardDisplay, IPointerClickHandler
{
    [SerializeField] List<Image> imageComponents;
    [SerializeField] List<TMP_Text> textComponents;
    bool fading;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (fading) return;
        // award or remove the card
        EventManager.clickedCard.Invoke(thisCard, gameObject);
    }

    readonly static float fadeDuration = 1f;
    readonly static Color32 fadeColor = new(1,1,1,0);
    public IEnumerator FadeAllComponents()
    {
        fading = true;
        float timeElapsed = 0;
        while(timeElapsed < fadeDuration)
        {
            foreach(var component in imageComponents)
            {
                component.color = Color32.Lerp(Color.white, fadeColor, timeElapsed / fadeDuration);
            }
            foreach (var component in textComponents)
            {
                component.color = Color32.Lerp(Color.white, fadeColor, timeElapsed / fadeDuration);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }
}
