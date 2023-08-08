using System;
using UnityEditor;
using UnityEngine;

namespace MekaruStudios.Tools
{
    public static class MonsterCreatorStyling
    {
        public static void CreatePopupSelection(ref int popupIndex, GUIContent[] guiContents, Action onPopupSelected)
        {
            EditorGUI.BeginChangeCheck();
            popupIndex = EditorGUILayout.Popup(popupIndex, guiContents);
            if (EditorGUI.EndChangeCheck())
            {
                onPopupSelected.Invoke();
            }
        }

        public static void CreateSquareButton(GUIContent guiContent, Action onClick)
        {
            if (GUILayout.Button(guiContent, GUILayout.Width(60), GUILayout.Height(60)))
                onClick.Invoke();
        }

        public static void CreateButton(GUIContent guiContent, Action onClick, Color? btnColor = null, params GUILayoutOption[] guiLayoutOptions)
        {
            var originalColor = GUI.color;
            if (btnColor != null) GUI.color = btnColor.Value;
            if (GUILayout.Button(guiContent, guiLayoutOptions))
                onClick.Invoke();
            GUI.color = originalColor;
        }

        public static void CreateListOfButtons(ref Vector2 scrollPos, GUIContent[] guiContents, Action<int> onClick, Action onEmptyBtnClick)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(72));
            EditorGUILayout.BeginHorizontal();

            CreateSquareButton(new GUIContent("Empty"), onEmptyBtnClick.Invoke);

            for (var i = 0; i < guiContents.Length - 1; i++)
            {
                CreateSquareButton(guiContents[i], () => onClick(i));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    }
}
