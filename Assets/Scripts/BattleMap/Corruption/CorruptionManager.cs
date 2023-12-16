using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionManager : MonoBehaviour
{
    [System.Serializable]
    struct CorruptionTier
    {
        public int tierThreshold;
        public CorruptionElement corruptionElement;
        public int corruptionBudget;
    }

    [SerializeField] List<CorruptionTier> corruptionTiers;
    [SerializeField] RunData runData;

    //need to figure out how to dynamically update corruption in world
    public void CorruptScene()
    {
        Dictionary<CorruptionElement, int> activeTiers = new();
        foreach(var tier in corruptionTiers)
        {
            if (runData.ThreatLevel < tier.tierThreshold) continue;

            if (!activeTiers.ContainsKey(tier.corruptionElement))
            {
                activeTiers.Add(tier.corruptionElement, tier.corruptionBudget);
            }
            else
            {
                activeTiers[tier.corruptionElement] = tier.corruptionBudget;
            }
        }

        foreach(var element in activeTiers.Keys)
        {
            element.Activate(activeTiers[element]);
        }
    }
}
