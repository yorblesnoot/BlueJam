using UnityEngine;

namespace MekaruStudios.MonsterCreatorTool
{
    public struct CosmeticAsset
    {
        public GameObject prefab;
        public string slotName;
        
        public CosmeticAsset(GameObject prefab, string slotName)
        {
            this.prefab = prefab;
            this.slotName = slotName;
        }
    }
}
