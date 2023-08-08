using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MekaruStudios.MonsterCreatorTool;
using UnityEditor;
using UnityEngine;

namespace MekaruStudios.Tools
{
    public class MonsterCreator : EditorWindow
    {
        public GameObject previewObject;
        public Material activeBaseMaterial;
        public Material activeEyeMaterial;
        public Material activeMouthMaterial;

        Editor previewEditor;


        Vector2 eyeScrollPos;
        Vector2 mouthScrollPos;


        // NEW


        XmlNode activeMonsterNode;

        int materialPopupIndex;



        GameObject currentCosmetic;
        Vector2 cosmeticScrollPos;

        SerializedObject so;
        SerializedProperty propPreviewObject;
        SerializedProperty propActiveBaseMaterial;
        SerializedProperty propActiveEyeMaterial;
        SerializedProperty propActiveMouthMaterial;

        Vector2 scrollPos;

        MaterialPropertyBlock mpb;
        public MaterialPropertyBlock Mpb => mpb ??= new MaterialPropertyBlock();

        static readonly int MainTex = Shader.PropertyToID("_MainTex");

        [MenuItem("Tools/Mekaru Studios/Monster Creator")]
        static void ShowWindow() => GetWindow<MonsterCreator>();

        #region Initialize

        void OnEnable()
        {
            Initialize();
            if(!MonsterCreatorObjects.isLoaded)
                LoadData();
        }
        
        void Initialize()
        {
            so = new SerializedObject(this);
            propPreviewObject = so.FindProperty("previewObject");
            propActiveBaseMaterial = so.FindProperty("activeBaseMaterial");
            propActiveEyeMaterial = so.FindProperty("activeEyeMaterial");
            propActiveMouthMaterial = so.FindProperty("activeMouthMaterial");
        }
        void LoadData()
        {
            LoadXMLFiles();
            GetEyeMaterials();
            GetMaterials();
            GetMouthMaterials();
            GetCosmetics();
            FindDiscordLogo();
            MonsterCreatorObjects.isLoaded = true;
        }
        void ResetData()
        {
            LoadData();
            DestroyPreview();
            MonsterCreatorObjects.activeXMLDoc = null;
            MonsterCreatorObjects.monsterTypes = null;
        }

        #endregion
        void FindDiscordLogo()
        {
            MonsterCreatorObjects.discordLogo = Resources.Load("Logo/discord_logo") as Texture2D;
        }
        void OnDisable()
        {
            DestroyPreview();
        }
        void DestroyPreview()
        {
            if (previewObject != null)
                DestroyImmediate(previewObject);
            if (previewEditor != null)
                DestroyImmediate(previewEditor);
        }
        void LoadXMLFiles()
        {
            //Reset XML files
            MonsterCreatorObjects.xmlDocuments.Clear();
            
            // Find a better way to find
            var guids = AssetDatabase.FindAssets("MekaruStudiosMonsterCreator");
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            foreach (var path in paths)
            {
                var doc = new XmlDocument();
                doc.Load(path);
                if(!MonsterCreatorObjects.xmlDocuments.Contains(doc))
                    MonsterCreatorObjects.xmlDocuments.Add(doc);
            }
        }
        void GetEyeMaterials()
        {
            MonsterCreatorObjects.eyeMaterials = Resources.LoadAll<Material>("Faces/Eyes");

            //Set GUI Contents
            MonsterCreatorObjects.eyeGUIContents = new GUIContent[MonsterCreatorObjects.eyeMaterials.Length + 1];
            for (var i = 0; i < MonsterCreatorObjects.eyeMaterials.Length; i++)
            {
                var mat = MonsterCreatorObjects.eyeMaterials[i];
                MonsterCreatorObjects.eyeGUIContents[i] = new GUIContent(mat.GetTexture(MainTex));
            }
        }
        void GetMouthMaterials()
        {
            MonsterCreatorObjects.mouthMaterials = Resources.LoadAll<Material>("Faces/Mouths");
            MonsterCreatorObjects.mouthGUIContents = new GUIContent[MonsterCreatorObjects.mouthMaterials.Length + 1];
            for (var i = 0; i < MonsterCreatorObjects.mouthMaterials.Length; i++)
            {
                var mat = MonsterCreatorObjects.mouthMaterials[i];
                MonsterCreatorObjects.mouthGUIContents[i] = new GUIContent(mat.GetTexture(MainTex));
            }
        }
        void GetCosmetics()
        {
            var cosmeticsList = new List<CosmeticAsset>();

            LoadCosmeticTypes("Hat");
            LoadCosmeticTypes("Wing");
            LoadCosmeticTypes("Body");
            MonsterCreatorObjects.cosmeticAssets = cosmeticsList.ToArray();

            MonsterCreatorObjects.cosmeticGUIContents = new GUIContent[MonsterCreatorObjects.cosmeticAssets.Length];
            for (var i = 0; i < MonsterCreatorObjects.cosmeticAssets.Length; i++)
            {
                var cosmeticPrefab = MonsterCreatorObjects.cosmeticAssets[i].prefab;
                var icon = AssetPreview.GetAssetPreview(cosmeticPrefab);
                MonsterCreatorObjects.cosmeticGUIContents[i] = new GUIContent(icon);
            }

            return;

            void LoadCosmeticTypes(string slotName)
            {
                var cosmeticPrefabs = Resources.LoadAll<GameObject>("Cosmetics/" + slotName);
                foreach (var prefab in cosmeticPrefabs)
                {
                    var cosmetic = new CosmeticAsset(prefab, slotName);
                    cosmeticsList.Add(cosmetic);
                }
            }
        }
        void GetMaterials()
        {
            MonsterCreatorObjects.meshMaterials = Resources.LoadAll<Material>("MonsterTool/Materials");
            MonsterCreatorObjects.meshMaterialGUIContents = new GUIContent[MonsterCreatorObjects.meshMaterials.Length];
            for (var i = 0; i < MonsterCreatorObjects.meshMaterialGUIContents.Length; i++)
            {
                var mat = MonsterCreatorObjects.meshMaterials[i];
                MonsterCreatorObjects.meshMaterialGUIContents[i] = new GUIContent(mat.name);
            }
        }
        void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            DrawLeftPane();
            DrawRightPane();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            return;

            void DrawPreviewEditor()
            {
                var bgColor = new GUIStyle
                {
                    normal =
                    {
                        background = EditorGUIUtility.whiteTexture
                    }
                };
                if (previewEditor == null)
                    previewEditor = Editor.CreateEditor(previewObject);

                previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
            }

            void RedrawPreviewEditor()
            {
                //Redraw the preview editor.
                DestroyImmediate(previewEditor);

                if (activeMonsterNode == null)
                    return;

                if (previewObject == null)
                    return;

                DrawBaseMaterial();
                DrawEyePlane();
                DrawMouthPlane();

                return;

                void DrawEyePlane()
                {
                    if (activeEyeMaterial != null)
                    {
                        var eyeObjectName = activeMonsterNode.SelectSingleNode("MATERIAL")?.SelectSingleNode("EYE")?.InnerText;
                        var eyeTransform = previewObject.transform.Find(eyeObjectName);
                        eyeTransform.gameObject.SetActive(true);
                        eyeTransform.GetComponent<SkinnedMeshRenderer>().material = activeEyeMaterial;
                    }
                    else
                    {
                        var eyeObjectName = activeMonsterNode.SelectSingleNode("MATERIAL")?.SelectSingleNode("EYE")?.InnerText;
                        var transform = previewObject.transform.Find(eyeObjectName);
                        transform.gameObject.SetActive(false);
                    }
                }

                void DrawBaseMaterial()
                {
                    if (activeBaseMaterial != null)
                    {
                        var rendererObjectName = activeMonsterNode.SelectSingleNode("MATERIAL")?.SelectSingleNode("BASE")?.InnerText;
                        previewObject.transform.Find(rendererObjectName).GetComponent<SkinnedMeshRenderer>().material = activeBaseMaterial;
                    }
                }

                void DrawMouthPlane()
                {
                    if (activeMouthMaterial != null)
                    {
                        var mouthObjectName = activeMonsterNode.SelectSingleNode("MATERIAL")?.SelectSingleNode("MOUTH")?.InnerText;
                        var transform = previewObject.transform.Find(mouthObjectName);
                        transform.gameObject.SetActive(true);
                        transform.GetComponent<SkinnedMeshRenderer>().material = activeMouthMaterial;
                    }
                    else
                    {
                        var mouthObjectName = activeMonsterNode.SelectSingleNode("MATERIAL")?.SelectSingleNode("MOUTH")?.InnerText;
                        var transform = previewObject.transform.Find(mouthObjectName);
                        transform.gameObject.SetActive(false);
                    }
                }
            }

            void CreateCosmeticsButtons()
            {

                MonsterCreatorStyling.CreateListOfButtons(ref cosmeticScrollPos, MonsterCreatorObjects.cosmeticGUIContents, (i) =>
                {
                    //Remove if already have cosmetic
                    if (currentCosmetic != null)
                        DestroyImmediate(currentCosmetic);

                    var cosmeticNode = activeMonsterNode.SelectSingleNode("COSMETICS");
                    var cosmeticPrefab = MonsterCreatorObjects.cosmeticAssets[i].prefab;
                    var slotName = MonsterCreatorObjects.cosmeticAssets[i].slotName.ToUpper(CultureInfo.InvariantCulture);
                    var slotNode = cosmeticNode.SelectSingleNode(slotName);
                    var instantiatePrefab = (GameObject)PrefabUtility.InstantiatePrefab(cosmeticPrefab);


                    var bonePath = slotNode.SelectSingleNode("BONE").InnerText;
                    var offsetString = slotNode.SelectSingleNode("OFFSET")?.InnerText;
                    var parts = offsetString.Split(',');

                    float x, y, z;
                    var successX = float.TryParse(parts[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out x);
                    var successY = float.TryParse(parts[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out y);
                    var successZ = float.TryParse(parts[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out z);
            

                    var bone = previewObject.transform.Find(bonePath);
                    instantiatePrefab.transform.SetParent(bone);
                    instantiatePrefab.transform.position = new Vector3(x, y, z);
                    currentCosmetic = instantiatePrefab;
                    DestroyImmediate(previewEditor);
                }, () =>
                {
                    //Remove if already have cosmetic
                    if (currentCosmetic != null)
                        DestroyImmediate(currentCosmetic);

                    DestroyImmediate(previewEditor);
                });
            }

            void CreateEyeButtons()
            {
                MonsterCreatorStyling.CreateListOfButtons(ref eyeScrollPos, MonsterCreatorObjects.eyeGUIContents, (i) =>
                {
                    activeEyeMaterial = MonsterCreatorObjects.eyeMaterials[i];
                    RedrawPreviewEditor();
                }, () =>
                {
                    activeEyeMaterial = null;
                    RedrawPreviewEditor();
                });
            }

            void CreateMouthButtons()
            {
                MonsterCreatorStyling.CreateListOfButtons(ref mouthScrollPos, MonsterCreatorObjects.mouthGUIContents, (i) =>
                {
                    activeMouthMaterial = MonsterCreatorObjects.mouthMaterials[i];
                    RedrawPreviewEditor();
                }, () =>
                {

                    activeMouthMaterial = null;
                    RedrawPreviewEditor();
                });
            }

            void CreateButtonsForMekaruAssets()
            {
                foreach (var doc in MonsterCreatorObjects.xmlDocuments)
                {
                    var titleNodes = doc.GetElementsByTagName("TITLE");
                    var label = titleNodes[0].InnerText;
                    if (GUILayout.Button(label))
                    {
                        MonsterCreatorObjects.activeXMLDoc = doc;
                        var activeTitle = MonsterCreatorObjects.activeXMLDoc.GetElementsByTagName("TITLE")[0].InnerText;
                        GetMonsterTypes();
                    }
                }
            }

            void DrawMonsterTypeButtons()
            {
                EditorGUILayout.BeginHorizontal();

                //Display buttons for creating slime types
                for (var i = 0; i < MonsterCreatorObjects.monsterTypes.Length; i++)
                {
                    var prefab = MonsterCreatorObjects.monsterTypes[i];
                    var icon = AssetPreview.GetAssetPreview(prefab);
                    if (GUILayout.Button(new GUIContent(icon)))
                    {
                        //First clear the preview editor
                        if (previewEditor != null)
                            DestroyImmediate(previewEditor);

                        activeBaseMaterial = null;

                        //Create a new prefab and log whether Prefab was saved successfully.
                        previewObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        previewObject.hideFlags = HideFlags.HideInHierarchy;

                        activeMonsterNode = MonsterCreatorObjects.activeXMLDoc.GetElementsByTagName("MONSTER")[i];
                    }
                }
                EditorGUILayout.EndHorizontal();
            }


            void DrawLeftPane()
            {
                GUILayout.BeginVertical("box", GUILayout.Width(200), GUILayout.MinHeight(Screen.height));
                GUILayout.Label("Left pane");

                CreateButtonsForMekaruAssets();
                MonsterCreatorStyling.CreateButton(new GUIContent("Reset"), ResetData, Color.red);

                GUILayout.EndVertical();
            }

            void DrawRightPane()
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("Right pane");

                if (MonsterCreatorObjects.monsterTypes != null)
                {

                    so.Update();
                    EditorGUILayout.PropertyField(propPreviewObject);

                    if (previewObject == null)
                    {
                        DrawMonsterTypeButtons();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propActiveBaseMaterial);
                        EditorGUILayout.PropertyField(propActiveEyeMaterial);
                        EditorGUILayout.PropertyField(propActiveMouthMaterial);

                        MonsterCreatorStyling.CreatePopupSelection(ref materialPopupIndex, MonsterCreatorObjects.meshMaterialGUIContents, () =>
                        {
                            activeBaseMaterial = MonsterCreatorObjects.meshMaterials[materialPopupIndex];
                            RedrawPreviewEditor();
                        });
                        CreateCosmeticsButtons();
                        CreateEyeButtons();
                        CreateMouthButtons();


                        DrawPreviewEditor();

                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button(new GUIContent("Discard"), new GUIStyle(GUI.skin.button)
                            {
                                fontStyle = FontStyle.Bold
                            }))
                        {
                            DestroyImmediate(previewObject);
                            previewObject = null;
                        }
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button(new GUIContent("Save The Monster"), new GUIStyle(GUI.skin.button)
                            {
                                fontStyle = FontStyle.Bold
                            }))
                        {
                            var absolutePath = EditorUtility.OpenFolderPanel("Select output path", "", "");
                            var localPath = "";
                            if (absolutePath.StartsWith(Application.dataPath))
                                localPath = "Assets" + absolutePath.Substring(Application.dataPath.Length);

                            if (previewObject != null)
                            {
                                localPath += "/" + previewObject.name + ".prefab";

                                //Make sure the file name is unique
                                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                                PrefabUtility.SaveAsPrefabAsset(previewObject, localPath, out var prefabSuccess);
                                if (prefabSuccess)
                                {
                                    if (previewEditor != null)
                                        DestroyImmediate(previewEditor);
                                    DestroyImmediate(previewObject);
                                }
                            }


                        }
                        GUI.backgroundColor = Color.white;
                    }
                    
                    DrawDiscordFooter();


                    if (so.ApplyModifiedProperties())
                        RedrawPreviewEditor();
                }
                GUILayout.EndVertical();
            }

        }
        void GetMonsterTypes()
        {
            var prefabs = new List<GameObject>();
            var monstersNodes = MonsterCreatorObjects.activeXMLDoc.GetElementsByTagName("MONSTER");
            foreach (XmlNode node in monstersNodes)
            {
                var modelName = node.SelectSingleNode("NAME")?.InnerText;
                var model = Resources.Load("MonsterTool/Types/" + modelName) as GameObject;
                prefabs.Add(model);
            }

            MonsterCreatorObjects.monsterTypes = prefabs.ToArray();
        }
        static void DrawDiscordFooter()
        {
                const float height = 40;
            if(MonsterCreatorObjects.discordLogo == null)
                return;
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.MaxHeight(height));
            {
              EditorGUILayout.HelpBox("Got a problem? Join Discord Now!", MessageType.Info);
                if (GUILayout.Button(new GUIContent(MonsterCreatorObjects.discordLogo), GUILayout.MaxHeight(height-1)))
                {
                    Application.OpenURL("https://discord.gg/Zf5kDUzYfq");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
