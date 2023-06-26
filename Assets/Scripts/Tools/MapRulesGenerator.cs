using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class MapRulesGenerator
{
    public static bool[,] Convert(TileMapShape mapShape, int radius, int gap)
    {
        int fullSize = 2 * radius + 1;
        bool[,] output = new bool[fullSize, fullSize];
        for (int x = 0; x < fullSize; x++)
        {
            for (int y = 0; y < fullSize; y++)
            {
                int xLength = Mathf.Abs(x - radius);
                int yLength = Mathf.Abs(y - radius);
                if (mapShape == TileMapShape.SQUARE)
                {
                    if (xLength >= gap || yLength >= gap) output[x, y] = true;
                    else output[x, y] = false;
                }
                else if(mapShape == TileMapShape.CIRCLE)
                {
                    int pathLength = xLength + yLength;
                    if (pathLength <= radius && pathLength >= gap) output[x, y] = true;
                    else output[x, y] = false;
                }
                else if(mapShape == TileMapShape.CROSS)
                {
                    if (x == radius && yLength <= radius && yLength >= gap) output[x, y] = true;
                    else if (y == radius && xLength <= radius && xLength >= gap) output[x, y] = true;
                    else output[x, y] = false;
                }
                else if( mapShape == TileMapShape.DIAGONALCROSS)
                {
                    if (xLength == yLength && xLength >= gap) output[x, y] = true;
                    else output[x, y] = false;
                }
            }
        }
        return output;
    }
}
