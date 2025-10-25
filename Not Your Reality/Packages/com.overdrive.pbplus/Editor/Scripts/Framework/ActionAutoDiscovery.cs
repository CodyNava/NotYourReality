using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Overdrive.ProBuilderPlus
{
    /// <summary>
    /// Auto-discovery system for ProBuilderPlus actions.<br/>
    /// Scans assemblies for actions with ProBuilderPlusActionAttribute and creates ActionInfo automatically.<br/>
    /// </summary>
    public static class ActionAutoDiscovery
    {
        /// <summary>
        /// Static collection of term that an assembly name starts with that are
        /// not containing <see cref="ProBuilderPlusActionAttribute"/> so they do not need to be checked.
        /// </summary>
        private static readonly string[] nonCheckAssemblies = new string[]
        {
            "Unity",
            "System",
            "Mono.",
            "mscorlib",
            "netstandard",
            "Newtonsoft",
            "nunit.framework",
            "Anonymously Hosted DynamicMethods Assembly",
            "Bee.BeeDriver2",
            "Bee.BinLog",
            "Bee.TinyProfiler2",
            "Domain_Reload",
            "ExCSS.Unity",
            "I18N",
            "I18N.West",
            "PlayerBuildProgramLibrary.Data",
            "PPv2URPConverters",
            "ScriptCompilationBuildProgram.Data",
            "WinPlayerBuildProgram.Data",
        };

        private static List<ActionInfo> s_CachedEdgeActions;
        private static List<ActionInfo> s_CachedEditorActions;
        private static List<ActionInfo> s_CachedFaceActions;
        private static List<ActionInfo> s_CachedObjectActions;
        private static List<ActionInfo> s_CachedVertexActions;

        /// <summary>
        /// Clear all cached actions (call when assemblies reload)
        /// </summary>
        public static void ClearCache()
        {
            s_CachedEditorActions = null;
            s_CachedObjectActions = null;
            s_CachedFaceActions = null;
            s_CachedEdgeActions = null;
            s_CachedVertexActions = null;
        }

        /// <summary>
        /// Get all actions valid for Edge selection mode
        /// </summary>
        public static List<ActionInfo> GetEdgeModeActions()
        {
            CreateCaches();
            return s_CachedEdgeActions;
        }

        /// <summary>
        /// Get all actions for editor mode (no selection mode restrictions)
        /// </summary>
        public static List<ActionInfo> GetEditorActions()
        {
            CreateCaches();
            return s_CachedEditorActions;
        }

        /// <summary>
        /// Get all actions valid for Face selection mode
        /// </summary>
        public static List<ActionInfo> GetFaceModeActions()
        {
            CreateCaches();
            return s_CachedFaceActions;
        }

        /// <summary>
        /// Get all actions valid for Object selection mode
        /// </summary>
        public static List<ActionInfo> GetObjectModeActions()
        {
            CreateCaches();
            return s_CachedObjectActions;
        }

        /// <summary>
        /// Get all actions valid for Vertex selection mode
        /// </summary>
        public static List<ActionInfo> GetVertexModeActions()
        {
            CreateCaches();
            return s_CachedVertexActions;
        }

        private static void CreateCaches()
        {
            if (s_CachedEditorActions != null)
                return;

            //// var time1 = System.Diagnostics.Stopwatch.GetTimestamp();
    
            var typeAttributePairs = DiscoverActions();

            s_CachedEditorActions = new List<ActionInfo>();
            s_CachedObjectActions = new List<ActionInfo>();
            s_CachedFaceActions = new List<ActionInfo>();
            s_CachedEdgeActions = new List<ActionInfo>();
            s_CachedVertexActions = new List<ActionInfo>();

            foreach (var valuePair in typeAttributePairs)
            {
                AddIfMatching(ToolMode.Object, ProBuilderPlusActionType.EditorPanel, valuePair, s_CachedEditorActions);
                AddIfMatching(ToolMode.Object, ProBuilderPlusActionType.Action, valuePair, s_CachedObjectActions);
                AddIfMatching(ToolMode.Face, ProBuilderPlusActionType.Action, valuePair, s_CachedFaceActions);
                AddIfMatching(ToolMode.Edge, ProBuilderPlusActionType.Action, valuePair, s_CachedEdgeActions);
                AddIfMatching(ToolMode.Vertex, ProBuilderPlusActionType.Action, valuePair, s_CachedVertexActions);
            }

            // Note: Sorting is deferred to ActionInfoProvider.CombineActions() to avoid double sorting

            static void AddIfMatching(
                ToolMode requestedMode,
                ProBuilderPlusActionType actionType,
                (Type Type, ProBuilderPlusActionAttribute ActionAttribute) valuePair,
                List<ActionInfo> actionsList)
            {
                // Check if this action is valid for the requested mode
                if ((valuePair.ActionAttribute.ValidModes & requestedMode) == 0)
                    return;

                // Check if this action is of the requested type
                if (valuePair.ActionAttribute.ActionType != actionType)
                    return;

                actionsList.Add(ActionInfo.CreateActionInfoFromAttribute(valuePair.Type, valuePair.ActionAttribute));
                return;
            }

            //// var time2 = System.Diagnostics.Stopwatch.GetTimestamp();
            //// Debug.LogFormat("Time Reflection: {0} ms", TimeSpan.FromTicks(time2 - time1).TotalMilliseconds);
        }

        /// <summary>
        /// Creates a list of (Type,ProBuilderPlusActionAttribute) pairs from reflection
        /// on all types of all loaded assemblies that ae not part of the exclusion-list.
        /// </summary>
        private static List<(Type Type, ProBuilderPlusActionAttribute ActionAttribute)> DiscoverActions()
        {
            var attributes = new List<(Type, ProBuilderPlusActionAttribute)>();

            // Get all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                if (IsNoAttributesAssembly(assembly.FullName))
                {
                    continue;
                }

                try
                {
                    var candidateTypes = assembly.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface);
                    foreach (var candidateType in candidateTypes)
                    {
                        var attribute = candidateType.GetCustomAttribute<ProBuilderPlusActionAttribute>();
                        if (attribute != null)
                        {
                            attributes.Add((candidateType, attribute));
                        }
                    }
                }
                catch (Exception e)
                {
                    // Skip assemblies that can't be reflected (like native assemblies)
                    Debug.LogWarning($"Could not scan assembly {assembly.FullName} for ProBuilderPlus actions: {e.Message}");
                }
            }

            return attributes;
        }

        /// <summary>
        /// Checks if a the given assembly name is in the list of assemblies that will not be checked for <see cref="ProBuilderPlusActionAttribute"/>.
        /// </summary>
        /// <param name="assemblyFullName">Fully qualified assembly name.</param>
        /// <returns>True if the assembly is to be skipped.</returns>
        private static bool IsNoAttributesAssembly(string assemblyFullName)
        {
            for (int index = 0; index < nonCheckAssemblies.Length; index++)
            {
                if (assemblyFullName.StartsWith(nonCheckAssemblies[index]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
