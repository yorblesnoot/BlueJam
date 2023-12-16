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

    public override void Activate(int _)
    {
        EventManager.playerAtWorldLocation.AddListener(UpdateFogGradient);
        fogRenderer.material.SetFloat(gradientProgression, GetProgression());
        lastCorruptionLevel = runData.ThreatLevel;
        animationLength /= 2;
    }

    void UpdateFogGradient(Vector2Int _)
    {
        if (runData.ThreatLevel > lastCorruptionLevel)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateFogChange());
            Debug.Log("update");
        }
        lastCorruptionLevel = runData.ThreatLevel;
    }

    [SerializeField] float animationLength;
    [SerializeField] float crestSize;
    IEnumerator AnimateFogChange()
    {
        float timeElapsed = 0;
        float start = lastCorruptionLevel / maxCorruption;
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
