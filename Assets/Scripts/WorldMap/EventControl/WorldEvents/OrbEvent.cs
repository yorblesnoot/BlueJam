using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbEvent : WorldEvent
{
    [SerializeField] CraftType type;
    [SerializeField] Color color;
    [SerializeField] GameObject core;

    string baseColor = "_BaseColor";
    string emissiveColor = "_EmissionColor";
    [SerializeField] int emissiveIntensity;

    private void OnEnable()
    {
        Material material = core.GetComponent<Renderer>().material;
        material.SetColor(baseColor, color);
        material.SetColor(emissiveColor, color * emissiveIntensity);
    }
    public override void Activate()
    {
        SoundManager.PlaySound(SoundType.GOTCHEST);
        EssenceCrafting.craftType = type;
        WorldMenuPlus.openAltCraft.Invoke();
        base.Activate();
    }
}
