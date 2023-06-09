using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CellTargeting
{
    public static List<GameObject> ConvertMapRuleToTiles(bool[,] targetData, Vector3 targetSource)
    {
        bool terrainBlocked = false;
        //find size of target data array
        List<GameObject> output = new();

        //size of target data
        int xLength = targetData.GetLength(0);
        int yLength = targetData.GetLength(1);

        //centerpoint of target data in local coords
        int xCenterpoint = xLength/2;
        int yCenterpoint = yLength/2;

        //global coordinates of the ability source
        Vector2Int sourceMap = MapTools.VectorToMap(targetSource);

        for(int x = 0; x < xLength; x++)
        {
            //y axis search
            for(int y = 0; y < yLength; y++)
            {
                if(targetData[x,y] != terrainBlocked)
                {
                    //difference between local centerpoint and local target cell
                    int xOffset = x - xCenterpoint;
                    int yOffset = y - yCenterpoint;

                    //get global coords for target cells
                    #nullable enable
                    GameObject? tile = MapTools.MapToTile(new Vector2Int(sourceMap[0] + xOffset, sourceMap[1] + yOffset));
                    #nullable disable

                    if(tile != null)
                    {
                        output.Add(tile);
                    }
                }
            }
        }
        return output;
    }

    public static List<GameObject> EliminateUnpathable(this List<GameObject> legalCells, GameObject targetSource)
    {
        int rangeSize = 20;
        List<Vector2Int> reducedLegals = legalCells.Select(a => MapTools.VectorToMap(a.transform.position)).ToList();
        bool[,] legalGrid = new bool[rangeSize,rangeSize];
        foreach (Vector2Int legalCell in reducedLegals)
        {
            if (TileIsValidTarget(MapTools.MapToTile(legalCell).GetComponent<BattleTileController>(), targetSource.tag, CardClass.MOVE))
            {
                legalGrid[legalCell.x, legalCell.y] = true;
            }
        }
        Vector2Int sourcePosition = MapTools.VectorToMap(targetSource.transform.position);

        reducedLegals = reducedLegals.Where(x => DoesPathExist(legalGrid, sourcePosition, x, true) == true).ToList();

        legalCells = reducedLegals.Select(a => MapTools.MapToTile(a)).ToList();
        return legalCells;
    }

    static bool DoesPathExist(bool[,] grid, Vector2Int current, Vector2Int destination, bool firstRun)
    {
        //if we are at the destination, return path
        if (current[0] == destination[0] && current[1] == destination[1])
        {
            if (grid[destination[0], destination[1]] == true) return true;
            else return false;

        }
        //if we've hit a blocked cell, return no path
        else if (firstRun == false && grid.Safe2DFind(current[0], current[1]) != true) return false;
        //if we are on an available cell, check for path on adjacents
        else
        {
            int directionX = Mathf.Clamp(destination[0] - current[0], -1, 1);
            if (directionX != 0)
            {
                //Debug.Log($"branch 1 to {destination[0]}, {destination[1]}");
                bool first = DoesPathExist(grid, new Vector2Int(current[0] + directionX, current[1]), destination, false);
                if (first == true) return true;
            }
            int directionY = Mathf.Clamp(destination[1] - current[1], -1, 1);
            if (directionY != 0)
            {
                //Debug.Log($"branch 2 to {destination[0]}, {destination[1]}");
                bool second = DoesPathExist(grid, new Vector2Int(current[0], current[1] + directionY), destination, false);
                if (second == true) return true;
            }
            //Debug.Log($"Child paths blocked {destination[0]}, {destination[1]}");
            return false;
        }
    }

    //return true if areatargets found valid plays
    public static bool ValidPlay(BattleTileController tile, BattleUnit source, CardPlus card)
    {
        string tSource = source.gameObject.tag;
        List<CardClass> classes = card.effects.Select(x => x.effectClass).ToList();
        if (classes.Contains(CardClass.MOVE) || classes.Contains(CardClass.SUMMON))
        {
            if (!TileIsValidTarget(tile, tSource, CardClass.MOVE)) return false;
            else return true;
        }
        foreach (var effect in card.effects)
        {
            if (effect.forceTargetSelf) tile = MapTools.VectorToTile(source.transform.position).GetComponent<BattleTileController>();
            if (effect.effectClass == CardClass.ATTACK && AreaTargets(tile.gameObject, tSource, CardClass.ATTACK, effect.aoe).Count == 0)
            {
                return false;
            }
            if (effect.effectClass == CardClass.BUFF && AreaTargets(tile.gameObject, tSource, CardClass.BUFF, effect.aoe).Count == 0)
            {
                return false;
            }
        }
        return true;
    }

    //return all valid targets in an aoe target based on the class, aoe size, and owner
    public static List<BattleUnit> AreaTargets(GameObject tile, string tSource, CardClass cardClass, bool[,] aoeRule)
    {
        List<GameObject> checkCells = ConvertMapRuleToTiles(aoeRule, tile.transform.position);
        List<BattleUnit> aoeTargets = new();
        if (checkCells.Count == 0) return aoeTargets;
        for (int i = 0; i < checkCells.Count; i++)
        {
            BattleTileController tileController = checkCells[i].GetComponent<BattleTileController>();
            if (TileIsValidTarget(tileController, tSource, cardClass))
            {
                BattleUnit cellContents = tileController.unitContents;
                if(cellContents != null) aoeTargets.Add(cellContents);
            }
        }
        return aoeTargets;
    }

    public static bool[,] CombineAOEIndicators(List<bool[,]> indicators)
    {
        List<int> sizes = indicators.Select(x => x.GetLength(0)).ToList();
        int maxSize = sizes.Max();
        List<int> offsets = sizes.Select(x => (maxSize - x) / 2).ToList();
        bool[,] output = new bool[maxSize, maxSize];
        for(int x = 0; x < maxSize; x++)
        {
            for(int y = 0; y < maxSize; y++)
            {
                bool cellValue = false;
                for (int i = 0; i < indicators.Count; i++)
                {
                    bool[,] indicator = indicators[i];
                    if (indicator.Safe2DFind(x - offsets[i],y - offsets[i]) ==true)
                    {
                        cellValue = true;
                        break;
                    }
                }
                output[x,y] = cellValue;
            }
        }
        return output;
    }

    public static bool TileIsValidTarget(BattleTileController tile, string tagOfSource, CardClass cardClass)
    {
        //logic to determine whether unit occupation makes the cell invalid
        #nullable enable
        BattleUnit? objTarget = tile.unitContents;
        #nullable disable

        string tagOfTarget;
        if (objTarget != null) tagOfTarget = objTarget.gameObject.tag;
        else tagOfTarget = "Empty";
        
        if(cardClass == CardClass.ATTACK)
        {
            //if theyre both allies or enemies, invalid
            if(tagOfSource == tagOfTarget) return false;
            //ally cant target player
            else if(tagOfSource == "Ally" && tagOfTarget == "Player") return false;
            //player cant target ally
            else if(tagOfSource == "Player" && tagOfTarget == "Ally") return false;
            //attack can't target empty
            else if(tagOfTarget == "Empty") return false;
            else return true;
        }
        else if(cardClass == CardClass.MOVE)
        {
            //move can only target empty
            if(tagOfTarget == "Empty") return true;
            else return false;
        }

        else if(cardClass == CardClass.SUMMON)
        {
            //summon can only target empty
            if(tagOfTarget == "Empty") return true;
            else return false;
        }

        else if(cardClass == CardClass.BUFF)
        {
            //buff can only target similar
            if(tagOfSource == tagOfTarget) return true;
            //empty is no good
            else if(tagOfTarget == "Empty") return false;
            else if(tagOfSource == "Ally" && tagOfTarget == "Player") return true;
            else if(tagOfSource == "Player" && tagOfTarget == "Ally") return true;
            else return false;
        }
        else return false;        
    }
}
