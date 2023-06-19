using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MekaruStudios.SlimePack
{


    public class SlimeCreator : EditorWindow
    {
        public GameObject previewObject;
        public Material material;
        public Material face;

        Editor previewEditor;

        GameObject[] slimeTypePrefabs;

        Material[] faceMaterials;
        GUIContent[] faceGUIContents;
        Vector2 faceScrollPos;



        Material[] slimeMaterials;
        GUIContent[] materialGUIContents;
        int materialPopupIndex;


        GameObject currentCosmetic;
        GameObject[] cosmetics;
        GUIContent[] cosmeticGUIContents;
        Vector2 cosmeticScrollPos;

        Texture2D[] discordLogo;

        SerializedObject so;
        SerializedProperty propPreviewObject;
        SerializedProperty propMaterial;
        SerializedProperty propFace;

        Vector2 scrollPos;

        MaterialPropertyBlock mpb;
        public MaterialPropertyBlock Mpb => mpb ??= new MaterialPropertyBlock();

        static readonly int MainTex = Shader.PropertyToID("_MainTex");

        [MenuItem("Tools/SlimeCreator")]
        static void ShowWindow() => GetWindow<SlimeCreator>();

        void OnEnable()
        {
            so = new SerializedObject(this);
            propPreviewObject = so.FindProperty("previewObject");
            propMaterial = so.FindProperty("material");
            propFace = so.FindProperty("face");

            GetAllSlimeTypeAssets();
            GetAllFaceTextures();
            GetAllSlimeMaterials();
            GetAllSlimeCosmetics();
            var guids = AssetDatabase.FindAssets("discord_logo", null);
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            discordLogo = paths.Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToArray();
        }

        void OnDisable()
        {
            if (previewObject != null)
                DestroyImmediate(previewObject);
            if (previewEditor != null)
                DestroyImmediate(previewEditor);

        }
        void GetAllFaceTextures()
        {
            var guids = AssetDatabase.FindAssets("l:SlimeFaceMaterials", null);
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            faceMaterials = paths.Select(AssetDatabase.LoadAssetAtPath<Material>).ToArray();
            faceGUIContents = new GUIContent[faceMaterials.Length + 1];

            //Set GUI Contents
            for (var i = 0; i < faceMaterials.Length; i++)
            {
                var mat = faceMaterials[i];
                faceGUIContents[i] = new GUIContent(mat.GetTexture(MainTex));
            }
        }
        void GetAllSlimeCosmetics()
        {
            //var guids = AssetDatabase.FindAssets($"t:{typeof(SlimeCosmeticData)}");
            var guids = AssetDatabase.FindAssets("[ACCESSORY]");
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            cosmetics = paths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();
            cosmeticGUIContents = new GUIContent[cosmetics.Length];

            //Set GUI Contents
            for (var i = 0; i < cosmetics.Length; i++)
            {
                var cosmetic = cosmetics[i];
                var icon = AssetPreview.GetAssetPreview(cosmetic);
                cosmeticGUIContents[i] = new GUIContent(icon);
            }
        }
        void GetAllSlimeMaterials()
        {
            var guids = AssetDatabase.FindAssets("l:SlimeMaterial", null);
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            slimeMaterials = paths.Select(AssetDatabase.LoadAssetAtPath<Material>).ToArray();
            materialGUIContents = new GUIContent[slimeMaterials.Length];
            for (var i = 0; i < materialGUIContents.Length; i++)
            {
                var mat = slimeMaterials[i];
                materialGUIContents[i] = new GUIContent(mat.name);
            }
        }
        void GetAllSlimeTypeAssets()
        {
            var guids = AssetDatabase.FindAssets("l:SlimeType", null);
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            slimeTypePrefabs = paths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();
        }
        void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            so.Update();
            EditorGUILayout.PropertyField(propPreviewObject);
            if (previewObject == null)
            {

                EditorGUILayout.BeginHorizontal();
                //Display buttons for creating slime types
                foreach (var prefab in slimeTypePrefabs)
                {
                    var icon = AssetPreview.GetAssetPreview(prefab);
                    if (GUILayout.Button(new GUIContent(icon)))
                    {
                        //First clear the preview editor
                        if (previewEditor != null)
                            DestroyImmediate(previewEditor);

                        material = null;

                        //Create a new prefab and log whether Prefab was saved successfully.
                        previewObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        previewObject.hideFlags = HideFlags.HideInHierarchy;

                    }
                }
                EditorGUILayout.EndHorizontal();


            }
            else
            {
                EditorGUILayout.PropertyField(propMaterial);
                EditorGUILayout.PropertyField(propFace);

                CreateMaterialPopup();
                CreateCosmeticsButtons();
                CreateFaceButtons();


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
                if (GUILayout.Button(new GUIContent("Save The Slime"), new GUIStyle(GUI.skin.button)
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


            if (so.ApplyModifiedProperties())
            {
                RedrawPreviewEditor();
            }
            EditorGUILayout.HelpBox("Got a problem? Join Discord Now!", MessageType.Info);
            if (GUILayout.Button(new GUIContent(discordLogo[0]), GUILayout.MaxHeight(32)))
            {
                Application.OpenURL("https://discord.gg/Zf5kDUzYfq");
            }

            GUILayout.EndScrollView();




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

                if (previewObject != null)
                {
                    if (material != null)
                        previewObject.transform.Find("SlimeBase").GetComponent<SkinnedMeshRenderer>().material = material;
                    if (face != null)
                    {
                        var faceTransform = previewObject.transform.Find("SlimeBase/Face");
                        faceTransform.gameObject.SetActive(true);
                        faceTransform.GetComponent<SkinnedMeshRenderer>().material = face;
                    }
                    else
                    {
                        previewObject.transform.Find("SlimeBase/Face").gameObject.SetActive(false);
                    }
                }

            }

            void CreateMaterialPopup()
            {
                EditorGUI.BeginChangeCheck();
                materialPopupIndex = EditorGUILayout.Popup(materialPopupIndex, materialGUIContents);
                if (EditorGUI.EndChangeCheck())
                {
                    material = slimeMaterials[materialPopupIndex];
                    RedrawPreviewEditor();
                }
            }

            void CreateCosmeticsButtons()
            {
                cosmeticScrollPos = EditorGUILayout.BeginScrollView(cosmeticScrollPos, GUILayout.Height(72));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Empty"), GUILayout.Height(60), GUILayout.Width(60)))
                {
                    //Remove if already have cosmetic
                    if (currentCosmetic != null)
                        DestroyImmediate(currentCosmetic);

                    DestroyImmediate(previewEditor);
                }
                for (var i = 0; i < cosmetics.Length; i++)
                {
                    if (GUILayout.Button(cosmeticGUIContents[i], GUILayout.Height(60), GUILayout.Width(60)))
                    {
                        //Remove if already have cosmetic
                        if (currentCosmetic != null)
                            DestroyImmediate(currentCosmetic);

                        var cosmetic = cosmetics[i];
                        var cosmeticPrefab = (GameObject)PrefabUtility.InstantiatePrefab(cosmetic);
                        var assetData = cosmetic.name.Split("_");
                        string boneName;
                        var heightOffset = 1f;

                        if (assetData.Length == 4)
                        {
                            boneName = assetData[^2];

                            //Find height offset
                            var y = assetData[^1];
                            y = y.Remove(0, 1);
                            y = y.Remove(y.Length - 1, 1);
                            heightOffset = float.Parse(y);
                        }
                        else
                        {
                            boneName = assetData[^1];
                        }

                        var bonePath = boneName switch
                        {
                            "[ROOT]" => "SlimeArmature/ROOT",
                            "[TOP]" => "SlimeArmature/ROOT/DEF_TOP",
                            "[BOTTOM]" => "SlimeArmature/ROOT/DEF_BOTTOM",
                            _ => ""
                        };

                        var bone = previewObject.transform.Find(bonePath);
                        cosmeticPrefab.transform.SetParent(bone);
                        cosmeticPrefab.transform.position = new Vector3(0, 1f, 0f) * heightOffset;
                        currentCosmetic = cosmeticPrefab;
                        DestroyImmediate(previewEditor);
                    }

                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

            }

            void CreateFaceButtons()
            {
                faceScrollPos = EditorGUILayout.BeginScrollView(faceScrollPos, GUILayout.Height(72));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Empty"), GUILayout.Width(60), GUILayout.Height(60)))
                {
                    face = null;
                    RedrawPreviewEditor();
                }
                for (var i = 0; i < faceMaterials.Length; i++)
                {
                    if (GUILayout.Button(faceGUIContents[i], GUILayout.Width(60), GUILayout.Height(60)))
                    {
                        face = faceMaterials[i];
                        RedrawPreviewEditor();
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
        }

    }
}
