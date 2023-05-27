using System.Collections.Generic;
using UnityEngine;

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
        float noiseFrequency = genParam.noiseFrequency;

        for (int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                float noiseX = x / (mapSize - .5f);
                float noiseY = y / (mapSize - .5f);
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
