using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGradient : CorruptionElement
{
    [SerializeField] Renderer fogRenderer;
    [SerializeField] float maxCorruption;
    [SerializeField] RunData runData;

    [SerializeField] int lastCorruptionLevel;

    string gradientProgression = "_GradientProgression";

    private void Awake()
    {
        fogRenderer.material.SetFloat(gradientProgression, GetProgression());
        animationLength /= 2;
    }

    public override void Activate(int _)
    {
        if (runData.ThreatLevel > lastCorruptionLevel)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateFogChange());
        }
        lastCorruptionLevel = runData.ThreatLevel;
    }

    [SerializeField] float animationLength;
    [SerializeField] float crestSize;
    IEnumerator AnimateFogChange()
    {
        float timeElapsed = 0;
        float start = lastCorruptionLevel / maxCorruption;
        Debug.Log(lastCorruptionLevel);
        float max = start + crestSize;
        while(timeElapsed < animationLength)
        {
            fogRenderer.material.SetFloat(gradientProgression, Mathf.Lerp(start, max, timeElapsed / animationLength));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        float final = GetProgression();
        while(timeElapsed > 0)
        {
            fogRenderer.material.SetFloat(gradientProgression, Mathf.Lerp(final, max, timeElapsed / animationLength));
            timeElapsed -= Time.deltaTime;
            yield return null;
        }
    }

    float GetProgression()
    {
        return Mathf.Clamp01(runData.ThreatLevel / maxCorruption);
    }
}
