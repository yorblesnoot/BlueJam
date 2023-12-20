using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleParticleSwitch : MonoBehaviour
{
    [ColorUsage(true,true)] [SerializeField] Color32 friendly;
    [ColorUsage(true, true)] [SerializeField] Color32 enemy;
    [SerializeField] Renderer ren;
    public void SetAllegiance(AllegianceType type)
    {
        Color32 chosen;
        if(type == AllegianceType.SLIME) chosen = enemy; else chosen = friendly;
        ren.material.SetColor(Shader.PropertyToID("_GlowColor"), chosen);
    }

}
