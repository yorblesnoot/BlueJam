using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIAnimator
{

    public static IEnumerator SlideIn(this GameObject window, float enterTime, float jiggleTime)
    {
        Transform transform = window.transform;
        float timeElapsed = 0;
        Vector3 startPosition = new(-Screen.width, 0, 0);
        window.transform.position = startPosition;
        window.SetActive(true);
        while (timeElapsed < enterTime)
        {
            Vector3 step = Vector3.Lerp(startPosition, Vector3.zero, timeElapsed / enterTime);
            timeElapsed += Time.deltaTime;
            transform.localPosition = step;
            yield return null;
        }
        transform.localPosition = Vector3.zero;

        timeElapsed = 0;
        Vector3 jiggleScale = new(.95f, 1, 1);
        float windowWidth = window.GetComponent<RectTransform>().rect.width;
        while (timeElapsed < jiggleTime)
        {
            Vector3 step = Vector3.Slerp(Vector3.one, jiggleScale, timeElapsed / jiggleTime);
            timeElapsed += Time.deltaTime;
            transform.localScale = step;
            transform.localPosition = new Vector3((windowWidth - (step.x * windowWidth)) * .5f, 0, 0);
            yield return null;
        }

        timeElapsed = 0;
        while (timeElapsed < jiggleTime)
        {
            Vector3 step = Vector3.Slerp(jiggleScale, Vector3.one, timeElapsed / jiggleTime);
            timeElapsed += Time.deltaTime;
            transform.localScale = step;
            transform.localPosition = new Vector3((windowWidth - (step.x * windowWidth)) * .5f, 0, 0);
            yield return null;
        }

    }

    public static IEnumerator SlideOut(this GameObject window, float exitTime)
    {
        Transform transform = window.transform;
        float timeElapsed = 0;
        Vector3 endPosition = new(Screen.width, 0, 0);
        while (timeElapsed < exitTime)
        {
            Vector3 step = Vector3.Lerp(Vector3.zero, endPosition, timeElapsed / exitTime);
            timeElapsed += Time.deltaTime;
            transform.localPosition = step;
            yield return null;
        }
        window.SetActive(false);
    }
}
