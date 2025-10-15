using _01_Scripts._04_Tools._01_Build_Tool;
using UnityEditor;
using UnityEngine;

namespace Editor
{
   [CustomEditor(typeof(BuildingToolObjects))]
   public class BuildingToolObjectsEditor : UnityEditor.Editor
   {
      private BuildingToolObjects _toolObjects;
      private const float IconSize = 80f;
      private void OnEnable() { _toolObjects = (BuildingToolObjects)target; }
      public override void OnInspectorGUI()
      {
         serializedObject.Update();

         SerializedProperty wallsProp = serializedObject.FindProperty("walls");
         SerializedProperty floorsProp = serializedObject.FindProperty("floors");
         SerializedProperty ceilingsProp = serializedObject.FindProperty("ceilings");
         SerializedProperty smallPropsProp = serializedObject.FindProperty("propsSmall");
         SerializedProperty mediumPropsProp = serializedObject.FindProperty("propsMedium");
         SerializedProperty largePropsProp = serializedObject.FindProperty("propsLarge");

         DrawPrefabSection("Walls", wallsProp);
         DrawPrefabSection("Floors", floorsProp);
         DrawPrefabSection("Ceilings", ceilingsProp);
         DrawPrefabSection("Small Props", smallPropsProp);
         DrawPrefabSection("Medium Props", mediumPropsProp);
         DrawPrefabSection("Large Props", largePropsProp);

         serializedObject.ApplyModifiedProperties();

         if (GUI.changed) EditorUtility.SetDirty(_toolObjects);
      }

      private void DrawPrefabSection(string title, SerializedProperty listProp)
      {
         EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

         if (GUILayout.Button($"Add {title} Prefab"))
         {
            listProp.arraySize++;
            listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue = null;
         }
         EditorGUILayout.Space(5);

         int columns = Mathf.Max(1, (int)(EditorGUIUtility.currentViewWidth / (IconSize + 10)));
         int index = 0;

         while (index < listProp.arraySize)
         {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < columns && index < listProp.arraySize; i++)
            {
               SerializedProperty element = listProp.GetArrayElementAtIndex(index);

               EditorGUILayout.BeginVertical(GUILayout.Width(IconSize));

               Texture2D preview = AssetPreview.GetAssetPreview(element.objectReferenceValue) ?? Texture2D.grayTexture;
               if (GUILayout.Button(preview, GUILayout.Width(IconSize), GUILayout.Height(IconSize)))
               {
                  Selection.activeObject = element.objectReferenceValue;
                  EditorGUIUtility.PingObject(element.objectReferenceValue);
               }

               element.objectReferenceValue = EditorGUILayout.ObjectField(
                  element.objectReferenceValue,
                  typeof(GameObject),
                  false,
                  GUILayout.Width(IconSize),
                  GUILayout.Height(16)
               );

               if (GUILayout.Button("X", GUILayout.Width(20)))
               {
                  listProp.DeleteArrayElementAtIndex(index);
                  break;
               }
               EditorGUILayout.EndVertical();
               index++;
            }
            EditorGUILayout.EndHorizontal();
         }
         EditorGUILayout.Space(10);
      }
   }
}