using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _01_Scripts._04_Tools._01_Build_Tool
{
   [InitializeOnLoad]
   public static class BuildingModeController
   {
      private static GameObject _selectedPrefab;
      private static GameObject _previewInstance;
      private static Material _ghostMaterial;

      private static bool _isActive;

      private static float _gridSize = 1f;
      private static float _rotationSnap = 15f;
      private static bool _snapping;

      private static SceneView _sceneView;

      static BuildingModeController() { SceneView.duringSceneGui += OnSceneGUI; }

      public static void SetSettings(float grid, float rotation, bool snapping)
      {
         _gridSize = grid;
         _rotationSnap = rotation;
         _snapping = snapping;
      }

      public static void StartGhostMode(GameObject prefab)
      {
         if (!prefab) { return; }

         _selectedPrefab = prefab;
         _isActive = true;

         if (_previewInstance) { Object.DestroyImmediate(_previewInstance); }

         _previewInstance = null;

         _sceneView = SceneView.lastActiveSceneView;
         if (_sceneView) { _sceneView.Focus(); }

         SceneView.RepaintAll();
      }

      private static void StopGhostMode()
      {
         _isActive = false;
         if (_previewInstance) { Object.DestroyImmediate(_previewInstance); }

         _previewInstance = null;
         _selectedPrefab = null;
         SceneView.RepaintAll();
      }

      private static void CreatePreview()
      {
         if (!_selectedPrefab) { return; }

         if (_previewInstance) { Object.DestroyImmediate(_previewInstance); }

         _previewInstance = Object.Instantiate(_selectedPrefab);
         _previewInstance.name = "[PREVIEW]";

         foreach (Renderer r in _previewInstance.GetComponentsInChildren<Renderer>())
         {
            Material[] mats = r.sharedMaterials;
            Material[] ghostMats = new Material[mats.Length];

            for (int i = 0; i < mats.Length; i++)
            {
               if (!mats[i])
               {
                  ghostMats[i] = null;
                  continue;
               }

               Material ghostMat = new Material(mats[i]);
               Color col = ghostMat.color;
               col.a = 0.5f;
               ghostMat.color = col;

               ghostMats[i] = ghostMat;
            }

            r.sharedMaterials = ghostMats;
         }

         foreach (Collider c in _previewInstance.GetComponentsInChildren<Collider>()) c.enabled = false;
      }

      private static void OnSceneGUI(SceneView sceneView)
      {
         Event e = Event.current;

         if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
         {
            StopGhostMode();
            e.Use();
            return;
         }

         if (!_isActive || !_selectedPrefab) { return; }

         Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

         Vector3 pos;
         if (Physics.Raycast(ray, out RaycastHit hit)) { pos = hit.point; }
         else
         {
            pos = ray.origin + ray.direction * 10f;
            pos.y = 0f;
         }

         if (_snapping)
         {
            pos.x = Mathf.Round(pos.x / _gridSize) * _gridSize;
            pos.z = Mathf.Round(pos.z / _gridSize) * _gridSize;
            pos.y = Mathf.Round(pos.y / _gridSize) * _gridSize;
         }

         if (!_previewInstance) { CreatePreview(); }

         if (_gridSize <= 0f) { return; }

         _previewInstance.transform.position = pos;

         if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
         {
            GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(_selectedPrefab);
            Undo.RegisterCreatedObjectUndo(placed, "Place Prefab");
            placed.transform.position = pos;
            placed.transform.rotation = _previewInstance.transform.rotation;
            Selection.activeGameObject = placed;
            e.Use();
         }

         if (e.type == EventType.KeyDown)
         {
            float angle;
            switch (e.keyCode)
            {
               case KeyCode.Alpha1:
                  angle = +_rotationSnap;
                  _previewInstance.transform.Rotate(Vector3.up, angle);
                  break;
               case KeyCode.Alpha2:
                  angle = -_rotationSnap;
                  _previewInstance.transform.Rotate(Vector3.up, angle);
                  break;
               case KeyCode.Alpha3:
                  angle = +_rotationSnap;
                  _previewInstance.transform.Rotate(Vector3.right, angle);
                  break;
               case KeyCode.Alpha4:
                  angle = -_rotationSnap;
                  _previewInstance.transform.Rotate(Vector3.right, angle);
                  break;
               case KeyCode.Alpha5: // Reset
                  _previewInstance.transform.eulerAngles = Vector3.zero;
                  break;
            }
            e.Use();
         }

         sceneView.Repaint();
      }
   }
}