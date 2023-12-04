using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleParticleSwitch : MonoBehaviour
{
    [SerializeField] Color32 friendly;
    [SerializeField] Color32 enemy;
    [SerializeField] ParticleSystem particle;
    public void SetAllegiance(AllegianceType type)
    {
        Color32 chosen;
        if(type == AllegianceType.SLIME) chosen = enemy; else chosen = friendly;
        ParticleSystem.MainModule mainMod = particle.main;
        mainMod.startColor = new ParticleSystem.MinMaxGradient(chosen, chosen);
    }

}
