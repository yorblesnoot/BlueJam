using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationParameters", menuName = "ScriptableObjects/MapKit/GenerationParameters")]
public class GenerationParameters : ScriptableObject
{
    public int mapSize;
    
    [Range(0f, 5f)]
    public float noiseFrequency;

    public List<string> symbolThresholds;
}
