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
        Pathfinder pathfinder = new();
        List<GameObject> output = new();
        foreach(GameObject cell in legalCells)
        {
            Vector2Int start = targetSource.ObjectToMap();
            Vector2Int end = cell.ObjectToMap();
            var path = pathfinder.FindObjectPath(start, end);
            int pathLength = path != null ? path.Count : 100;
            Vector2Int displacement = end - start;
            int taxiLength = Mathf.Abs(displacement.x) + Mathf.Abs(displacement.y);
            if(pathLength == taxiLength) output.Add(cell);
        }
        return output;
    }

    //return true if areatargets found valid plays
    public static bool ValidPlay(BattleTileController tile, BattleUnit source, CardPlus card)
    {
        AllegianceType sourceAllegiance = source.Allegiance;
        List<CardClass> classes = card.effects.Select(x => x.effectClass).ToList();
        foreach (var effect in card.effects)
        {
            BattleTileController effectTile = tile;
            if (effect.effectClass == CardClass.MOVE && effectTile.unitContents == null && !tile.IsRift) return true;
            if (effect.targetNotRequired || effect.forceTargetSelf) continue;
            int validTargets = AreaTargets(effectTile.gameObject, sourceAllegiance, effect.effectClass, effect.aoe).Count;
            if (validTargets == 0) return false;
        }
        return true;
    }

    //return all valid targets in an aoe target based on the class, aoe size, and owner
    public static List<BattleTileController> AreaTargets(GameObject tile, AllegianceType sourceAllegiance, CardClass cardClass, bool[,] aoeRule)
    {
        List<GameObject> checkCells = ConvertMapRuleToTiles(aoeRule, tile.transform.position);
        List<BattleTileController> aoeTargets = new();
        if (checkCells.Count == 0) return aoeTargets;
        for (int i = 0; i < checkCells.Count; i++)
        {
            BattleTileController tileController = checkCells[i].GetComponent<BattleTileController>();
            if (TileIsValidTarget(tileController, sourceAllegiance, cardClass))
            {
                aoeTargets.Add(tileController);
            }
        }
        return aoeTargets;
    }

    public static bool[,] CombineAOEIndicators(List<bool[,]> indicators)
    {
        List<int> sizes = indicators.Select(x => x.GetLength(0)).ToList();
        if(sizes.Count == 0) return new bool[,] { { true } };
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

    public static bool TileIsValidTarget(BattleTileController tile, AllegianceType sourceAllegiance, CardClass cardClass)
    {
        //logic to determine whether unit occupation makes the cell invalid
        BattleUnit objTarget = tile.unitContents;
        AllegianceType targetAllegiance;
        if (objTarget != null)
        {
            targetAllegiance = objTarget.Allegiance;
            bool friendly = FactionLogic.CheckIfFriendly(sourceAllegiance, targetAllegiance);
            if (friendly && cardClass == CardClass.BUFF) return true;
            if (!friendly && cardClass == CardClass.ATTACK) return true;
        }
        else
        {
            if (cardClass == CardClass.MOVE || cardClass == CardClass.SUMMON)
            {
                //move can only target empty
                if (tile.IsRift) return false;
                else return true;
            }
        }
        return false;        
    }
}
