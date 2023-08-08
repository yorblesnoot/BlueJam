using System.Collections.Generic;
using System.Xml;
using MekaruStudios.MonsterCreatorTool;
using UnityEngine;

namespace MekaruStudios.Tools
{

    public static class MonsterCreatorObjects
    {
        public static bool isLoaded;
        public static GameObject[] monsterTypes;
        public static Material[] eyeMaterials;
        public static Material[] mouthMaterials;
        public static GUIContent[] eyeGUIContents;
        public static GUIContent[] mouthGUIContents;
        public static Material[] meshMaterials;
        public static GUIContent[] meshMaterialGUIContents;
        public static GUIContent[] cosmeticGUIContents;
        public static CosmeticAsset[] cosmeticAssets;
        public static List<XmlDocument> xmlDocuments = new List<XmlDocument>();
        public static Texture2D discordLogo;

        public static XmlDocument activeXMLDoc;

        public static void Reset() => isLoaded = false;
    }
}
