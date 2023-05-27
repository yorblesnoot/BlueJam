using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class CellQuery : UnityEvent<int, int, string, GameObject> { }

public static class ZoneTargeter 
{
    //public CellQuery cellQuery;

    public static List<GameObject> CheckLegal(string[,] targetData, Vector3 targetSource)
    {
        string terrainBlocked = "x";
        //find size of target data array
        List<GameObject> output = new List<GameObject>();

        //size of target data
        int xLength = targetData.GetLength(0);
        int yLength = targetData.GetLength(1);

        //centerpoint of target data in local coords
        int xCenterpoint = xLength/2;
        int yCenterpoint = yLength/2;

        //global coordinates of the ability source
        int[] sourceMap = GridTools.VectorToMap(targetSource);

        for(int x = 0; x < xLength; x++)
        {
            //y axis search
            for(int y = 0; y < yLength; y++)
            {
                
                //cellQuery?.Invoke(x, y, targetData[x,y], owner);
                if(targetData[x,y] != terrainBlocked)
                {
                    //difference between local centerpoint and local target cell
                    int xOffset = x - xCenterpoint;
                    int yOffset = y - yCenterpoint;

                    //get global coords for target cells
                    #nullable enable
                    GameObject? tile = GridTools.VectorToTile(GridTools.MapToVector(sourceMap[0] + xOffset, sourceMap[1] + yOffset, 0f));
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

    //return true if areatargets found valid plays
    public static bool ValidPlay(GameObject tile, string tSource, List<CardClass> cardClass, string[,] aoeRule)
    {
        if (cardClass.Contains(CardClass.MOVE) || cardClass.Contains(CardClass.SUMMON))
        {
            if (ValidContents(tile, tSource, CardClass.MOVE) == true) return true;
            else return false;
        }
        else if(cardClass.Contains(CardClass.ATTACK) && AreaTargets(tile, tSource, CardClass.ATTACK, aoeRule).Count > 0)
        {
            return true;
        }
        else if (cardClass.Contains(CardClass.BUFF) && AreaTargets(tile, tSource, CardClass.BUFF, aoeRule).Count > 0)
        {
            return true;
        }
        else return false;
    }

    //return all valid targets in an aoe target based on the class, aoe size, and owner
    public static List<GameObject> AreaTargets(GameObject tile, string tSource, CardClass cardClass, string[,] aoeRule)
    {
        List<GameObject> checkCells = CheckLegal(aoeRule, tile.transform.position);
        List<GameObject> outCells = new List<GameObject>();
        if (checkCells.Count > 0)
        {
            for (int i = 0; i < checkCells.Count; i++)
            {
                if (ValidContents(checkCells[i], tSource, cardClass))
                {
                    BattleTileController tileControl = checkCells[i].GetComponent<BattleTileController>();
                    outCells.Add(tileControl.unitContents);
                }
            }
            return outCells;
        }
        else return null;
    }

    public static bool ValidContents(GameObject tile, string tSource, CardClass cardClass)
    {
        //logic to determine whether unit occupation makes the cell invalid
        #nullable enable
        GameObject? objTarget = tile.GetComponent<BattleTileController>().unitContents;
        #nullable disable

        string tTarget;
        if (objTarget != null) tTarget = objTarget.tag;
        else tTarget = "Empty";
        
        if(cardClass == CardClass.ATTACK)
        {
            //if theyre both allies or enemies, invalid
            if(tSource == tTarget) return false;
            //ally cant target player
            else if(tSource == "Ally" && tTarget == "Player") return false;
            //player cant target ally
            else if(tSource == "Player" && tTarget == "Ally") return false;
            //attack can't target empty
            else if(tTarget == "Empty") return false;
            else return true;
        }
        else if(cardClass == CardClass.MOVE)
        {
            //move can only target empty
            if(tTarget == "Empty") return true;
            else return false;
        }

        else if(cardClass == CardClass.SUMMON)
        {
            //summon can only target empty
            if(tTarget == "Empty") return true;
            else return false;
        }

        else if(cardClass == CardClass.BUFF)
        {
            //buff can only target similar
            if(tSource == tTarget) return true;
            //empty is no good
            else if(tTarget == "Empty") return false;
            else if(tSource == "Ally" && tTarget == "Player") return true;
            else if(tSource == "Player" && tTarget == "Ally") return true;
            else return false;
        }
        else return false;        
    }
}
