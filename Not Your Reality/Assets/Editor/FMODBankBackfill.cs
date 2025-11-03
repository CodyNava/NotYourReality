#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class FMODBankBackfill
    {
        private static readonly string SrcRoot = "Assets/StreamingAssets/FMOD";
        private static readonly string DstRoot = "Audio/FMOD/Build";

        static FMODBankBackfill()
        {
            EditorApplication.update += OneShot;
            EditorApplication.playModeStateChanged += s =>
            {
                if (s == PlayModeStateChange.ExitingEditMode)
                    EnsureBackfill(showDialogs: false);
            };
        }

        private static void OneShot()
        {
            EditorApplication.update -= OneShot;
            EnsureBackfill(showDialogs: false);
        }

        [MenuItem("FMOD/Backfill Project Build (from StreamingAssets)")]
        public static void BackfillMenu() => EnsureBackfill(showDialogs: true);

        [MenuItem("FMOD/Show Bank Status")]
        public static void ShowStatus()
        {
            var (srcAbs, dstAbs, srcCount, dstCount) = Count();
            Debug.Log($"[FMOD] Status → SRC: {srcAbs} ({srcCount} .bank) | DST: {dstAbs} ({dstCount} .bank)");
        }

        private static void EnsureBackfill(bool showDialogs)
        {
            var (srcAbs, dstAbs, srcCount, dstCount) = Count();
            
            if (srcCount == 0)
            {
                var msg = $"No .bank files under:\n{srcAbs}\n\n" +
                          "Build in FMOD Studio (File → Build). " +
                          "Build path should be the folder 'Assets/StreamingAssets/FMOD' (FMOD adds 'Desktop').";
                if (showDialogs) EditorUtility.DisplayDialog("FMOD Banks missing", msg, "OK");
                else Debug.LogWarning("[FMOD] " + msg.Replace("\n", " "));
                return;
            }
            
            if (dstCount == 0)
            {
                var copied = CopyDesktopBanks(srcAbs, dstAbs);
                Debug.Log($"[FMOD] Backfilled {copied} bank(s) into {Path.Combine(dstAbs, "Desktop")}.");
                return;
            }
        }

        private static (string srcAbs, string dstAbs, int srcCount, int dstCount) Count()
        {
            var srcAbs = Path.GetFullPath(Path.Combine(Application.dataPath, "..", SrcRoot));
            var dstAbs = Path.GetFullPath(Path.Combine(Application.dataPath, "..", DstRoot));
            var srcCount = Directory.Exists(srcAbs)
                ? Directory.GetFiles(srcAbs, "*.bank", SearchOption.AllDirectories).Length
                : 0;
            var dstCount = Directory.Exists(dstAbs)
                ? Directory.GetFiles(dstAbs, "*.bank", SearchOption.AllDirectories).Length
                : 0;
            return (srcAbs, dstAbs, srcCount, dstCount);
        }

        private static int CopyDesktopBanks(string srcAbs, string dstAbs)
        {
            var srcDesktop = Path.Combine(srcAbs, "Desktop");
            var dstDesktop = Path.Combine(dstAbs, "Desktop");
            Directory.CreateDirectory(dstDesktop);

            int copied = 0;
            if (!Directory.Exists(srcDesktop)) return 0;

            foreach (var srcFile in Directory.GetFiles(srcDesktop, "*.bank", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(srcDesktop, srcFile);
                var dstFile = Path.Combine(dstDesktop, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dstFile)!);
                File.Copy(srcFile, dstFile, true);
                copied++;
            }
            return copied;
        }
    }
}
#endif
