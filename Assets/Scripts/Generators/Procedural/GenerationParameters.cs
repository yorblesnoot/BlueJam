using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationParameters", menuName = "ScriptableObjects/GenerationParameters")]
public class GenerationParameters : ScriptableObject
{
    public int mapSize;
    
    [Range(0f, 5f)]
    public float noiseFrequency;

    public List<string> symbolThresholds;
}
