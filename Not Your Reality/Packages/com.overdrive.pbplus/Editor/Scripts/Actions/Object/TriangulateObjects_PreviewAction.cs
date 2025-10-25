using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
using Overdrive.ProBuilderPlus;

namespace Overdrive.ProBuilderPlus
{
    /// <summary>
    /// Triangulate Objects action with live wireframe preview.
    /// Shows preview of triangulation result before applying changes.
    /// </summary>
    //[ProBuilderMenuAction]
    [ProBuilderPlusAction("triangulate_objects_preview", "Triangulate",
        Tooltip = "Triangulate objects with live preview - shows cyan wireframe of triangle edges",
        Instructions = "Cyan lines show new edges to be created.",
        IconPath = "Icons/Old/Object_Triangulate",
        ValidModes = ToolMode.Object,
        Order = 53)]
    public class TriangulateObjectsPreviewAction : PreviewMenuAction
    {
        // Preview state
        private ProBuilderMesh[] m_CachedMeshes;
        private TriangulateHelper.TriangulationPreview[] m_TriangulationPreviews;

        public override ToolbarGroup group { get { return ToolbarGroup.Object; } }

        public override bool enabled
        {
            get { return base.enabled && MeshSelection.selectedObjectCount > 0; }
        }

        public override VisualElement CreateSettingsContent()
        {
            return TriangulateHelper.CreateTriangulateUI(false); // false = object mode
        }

        internal override void StartPreview()
        {
            // Cache the current selection
            var selection = MeshSelection.top.ToArray();
            m_CachedMeshes = selection;
            
            // Calculate previews using helper
            m_TriangulationPreviews = new TriangulateHelper.TriangulationPreview[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                m_TriangulationPreviews[i] = TriangulateHelper.CalculateTriangulationPreview(selection[i]);
            }

            // Subscribe to scene GUI for preview visualization
            SceneView.duringSceneGui += OnSceneGUI;

            // Initial preview update
            UpdatePreview();

        }

        internal override void UpdatePreview()
        {
            if (m_CachedMeshes == null) return;

            // Triangulation has no parameters to update, but we can recalculate if needed
            // For now, just repaint to show the preview
            SceneView.RepaintAll();
        }

        internal override ActionResult ApplyChanges()
        {
            
            if (m_CachedMeshes == null || m_CachedMeshes.Length == 0)
                return new ActionResult(ActionResult.Status.Failure, "No Objects Selected");

            Undo.RecordObjects(m_CachedMeshes, "Triangulate Objects");

            int totalFacesTriangulated = 0;
            
            // Apply triangulation to each mesh using helper
            foreach (var mesh in m_CachedMeshes)
            {
                if (mesh != null)
                {
                    totalFacesTriangulated += TriangulateHelper.ApplyTriangulation(mesh);
                }
            }

            var c = m_CachedMeshes.Length;
            string message = "Triangulate " + c + (c > 1 ? " Objects" : " Object") + $" ({totalFacesTriangulated} faces)";
            return new ActionResult(ActionResult.Status.Success, message);
        }

        internal override void CleanupPreview()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            m_CachedMeshes = null;
            m_TriangulationPreviews = null;
            SceneView.RepaintAll();
            
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            DrawWireframePreviews();
        }

        private void DrawWireframePreviews()
        {
            if (m_CachedMeshes == null || m_TriangulationPreviews == null) return;
            
            TriangulateHelper.DrawTriangulationPreviews(m_TriangulationPreviews);
        }
    }
}