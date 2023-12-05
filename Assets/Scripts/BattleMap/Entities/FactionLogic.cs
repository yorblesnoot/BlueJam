using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionLogic : MonoBehaviour
{
    [System.Serializable]
    class Relationship
    {
        public AllegianceType entity1;
        public AllegianceType entity2;
        public bool friendly;
    }

    [SerializeField] List<Relationship> relationships;

    static Dictionary<Vector2Int, bool> relationshipBook;

    private void Awake()
    {
        relationshipBook = GenerateRelationshipBook();
    }

    Dictionary<Vector2Int, bool> GenerateRelationshipBook()
    {
        Dictionary<Vector2Int, bool> output = new();
        foreach(var relationship in relationships)
        {
            output.Add(AllegiancesToVector(relationship.entity1, relationship.entity2), relationship.friendly);
            output.Add(AllegiancesToVector(relationship.entity2, relationship.entity1), relationship.friendly);
        }
        return output;
    }

    static Vector2Int AllegiancesToVector(AllegianceType type1, AllegianceType type2)
    {
        return new Vector2Int((int)type1, (int)type2);
    }

    public static bool CheckIfFriendly(AllegianceType type1, AllegianceType type2)
    {
        if(type1 == type2) return true;
        if (!relationshipBook.TryGetValue(AllegiancesToVector(type1, type2), out bool friendly))
            Debug.LogError("Unregistered allegiance interaction.");
        return friendly;

    }
}
