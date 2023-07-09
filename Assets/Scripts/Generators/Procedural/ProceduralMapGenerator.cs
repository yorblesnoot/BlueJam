using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralMapGenerator
{
    public string[,] Generate(GenerationParameters genParam)
    {
        int mapSize = genParam.mapSize;
        string[,] noiseMap = new string[mapSize, mapSize];

        //sampling point, number might need to be higher
        int randMax = 999999;
        float xOrg = Random.Range(0, randMax);
        float yOrg = Random.Range(0, randMax);

        //frequency: around 1? is best
        float noiseFrequency = genParam.noiseFrequency/10;

        for (int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                
                float noiseX = .99f * x;
                float noiseY = .99f * y;
                noiseX += xOrg;
                noiseY += yOrg;
                noiseX *= noiseFrequency;
                noiseY *= noiseFrequency;
                float noise = Mathf.PerlinNoise(noiseX, noiseY);
                noiseMap[x, y] = TranslateNoise(noise, genParam);
            }
        }
        return noiseMap;
    }

    public string TranslateNoise(float noise, GenerationParameters genParam)
    {
        List<string> symbolThresholds = genParam.symbolThresholds;
        int numSymbols = symbolThresholds.Count;
        for(int entry = 0; entry < numSymbols; entry++)
        {
            float threshold = (float)(entry + 1)/numSymbols;
            if (noise < threshold)
            {
                return symbolThresholds[entry];
            }
        }
        return symbolThresholds[numSymbols - 1];
    }
}
