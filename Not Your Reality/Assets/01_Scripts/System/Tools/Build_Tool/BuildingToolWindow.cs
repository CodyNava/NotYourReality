#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace System.Tools.Build_Tool
{
   public class BuildingToolWindow : EditorWindow
   {
      private BuildingToolObjects _toolObjects;
      private Vector2 _scrollPos;
      private readonly string[] _tabs = { "Walls", "Floors", "Ceilings", "Small Props", "Medium Props", "Large Props" };
      private int _currentTab;

      private const float IconSize = 80f;

      private float _gridSize = 1f;
      private float _rotationSnap = 15f;
      private bool _snappingXZ;
      private bool _snappingY;

      [MenuItem("Tools/Building Tool")]
      public static void OpenWindow() => GetWindow<BuildingToolWindow>("Building Tool");

      private void OnEnable()
      {
         string[] guids = AssetDatabase.FindAssets("t:BuildingToolObjects");
         if (guids.Length > 0)
         {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _toolObjects = AssetDatabase.LoadAssetAtPath<BuildingToolObjects>(path);
         }
      }

      private void OnGUI()
      {
         if (!_toolObjects)
         {
            GUILayout.Label("No BuildingToolObjects found.");
            if (GUILayout.Button("Select Asset"))
               _toolObjects = (BuildingToolObjects)EditorGUILayout.ObjectField(
                  "Asset",
                  _toolObjects,
                  typeof(BuildingToolObjects),
                  false
               );
            return;
         }

         _currentTab = GUILayout.Toolbar(_currentTab, _tabs);

         _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
         DrawCategory(_tabs[_currentTab]);
         EditorGUILayout.EndScrollView();

         DrawSettings();
      }

      private void DrawSettings()
      {
         GUILayout.Space(10);
         GUILayout.Label("Ghost Mode Settings", EditorStyles.boldLabel);

         _snappingXZ = EditorGUILayout.Toggle("SnappingHorizontal Toggle", _snappingXZ);
         _snappingY = EditorGUILayout.Toggle("SnappingVertical Toggle", _snappingY);
         _gridSize = EditorGUILayout.FloatField("Grid Size", _gridSize);
         _rotationSnap = EditorGUILayout.FloatField("Rotation Snap", _rotationSnap);

         BuildingModeController.SetSettings(_gridSize, _rotationSnap, _snappingXZ, _snappingY);
      }

      private void DrawCategory(string category)
      {
         List<GameObject> list = category switch
         {
            "Walls"        => _toolObjects.walls,
            "Floors"       => _toolObjects.floors,
            "Ceilings"     => _toolObjects.ceilings,
            "Small Props"  => _toolObjects.propsSmall,
            "Medium Props" => _toolObjects.propsMedium,
            "Large Props"  => _toolObjects.propsLarge,
            _              => null
         };

         if (list == null || list.Count == 0) return;

         int columns = Mathf.Max(1, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / (IconSize + 20)));
         int index = 0;

         while (index < list.Count)
         {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < columns && index < list.Count; i++)
            {
               GameObject prefab = list[index];
               if (!prefab)
               {
                  index++;
                  continue;
               }

               EditorGUILayout.BeginVertical(GUILayout.Width(IconSize), GUILayout.Height(IconSize + 20));

               Texture2D preview = AssetPreview.GetAssetPreview(prefab) ?? Texture2D.grayTexture;
               Rect rect = GUILayoutUtility.GetRect(IconSize, IconSize);
               GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);

               if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
               {
                  BuildingModeController.StartGhostMode(prefab);
                  Event.current.Use();
               }

               GUILayout.Label(prefab.name, EditorStyles.miniLabel, GUILayout.Width(IconSize), GUILayout.Height(20));

               EditorGUILayout.EndVertical();
               index++;
            }

            EditorGUILayout.EndHorizontal();
         }
      }
   }
}
#endif
