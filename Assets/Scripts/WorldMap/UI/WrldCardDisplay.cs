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
    class ColorImage
    {
        public Image image;
        public Color startColor;
    }

    class ColorText
    {
        public TMP_Text text;
        public Color startColor;
    }

    List<ColorImage> images;
    List<ColorText> texts;

    bool fading;

    private void Awake()
    {
        images = new();
        texts = new();
        foreach (var image in imageComponents)
        {
            images.Add(new ColorImage { image = image, startColor = image.color });
        }

        foreach (var text in textComponents)
        {
            texts.Add(new ColorText { text = text, startColor = text.color });
        }
    }
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
            foreach(var component in images)
            {
                component.image.color = Color32.Lerp(component.startColor, fadeColor, timeElapsed / fadeDuration);
            }
            foreach (var component in texts)
            {
                component.text.color = Color32.Lerp(component.startColor, fadeColor, timeElapsed / fadeDuration);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }

    private void OnDisable()
    {
        ResetColors();
    }

    public void ResetColors()
    {
        foreach (var component in images)
        {
            component.image.color = component.startColor;
        }
        foreach (var component in texts)
        {
            component.text.color = component.startColor;
        }
    }
}
