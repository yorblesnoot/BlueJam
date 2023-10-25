using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimateDraggable : MonoBehaviour
{
    [SerializeField] float fadeDuration;
    [SerializeField] GameObject particleFlow;
    [SerializeField] GameObject iconVisual;

    IEnumerator FadeAllOut()
    {
        particleFlow.SetActive(true);
        particleFlow.transform.SetAsLastSibling();
        float timeElapsed = 0;
        Vector3 startScale = iconVisual.transform.localScale;
        while (timeElapsed < fadeDuration)
        {
            iconVisual.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timeElapsed/fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
