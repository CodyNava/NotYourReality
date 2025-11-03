#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class FMODBankBackfill
    {
        private static readonly string SrcRoot = "Assets/StreamingAssets/FMOD";
        private static readonly string DstRoot = "Audio/FMOD/Build";
        
        private const bool DeleteStaleBanks = true;   
        private const bool VerboseLogs = true;      

        static FMODBankBackfill()
        {
            EditorApplication.update += OneShot;
        }

        private static void OneShot()
        {
            EditorApplication.update -= OneShot;
            SyncBanks(showDialogs: false);
            
        }

        [MenuItem("FMOD/Backfill Project Build (from StreamingAssets)")]
        public static void BackfillMenu() => SyncBanks(showDialogs: true);

        [MenuItem("FMOD/Show Bank Status")]
        public static void ShowStatus()
        {
            var (srcAbs, dstAbs, srcCount, dstCount) = Count();
            Debug.Log($"[FMOD] Status → SRC: {srcAbs} ({srcCount} .bank) | DST: {dstAbs} ({dstCount} .bank)");
        }

        private static void SyncBanks(bool showDialogs)
        {
            var (srcAbs, dstAbs, srcCount, dstCount) = Count();
            var srcDesktop = Path.Combine(srcAbs, "Desktop");
            var dstDesktop = Path.Combine(dstAbs, "Desktop");

            if (srcCount == 0 || !Directory.Exists(srcDesktop))
            {
                return;
            }

            Directory.CreateDirectory(dstDesktop);

            int added = 0, updated = 0, deleted = 0;
            
            foreach (var srcFile in Directory.GetFiles(srcDesktop, "*.bank", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(srcDesktop, srcFile);
                var dstFile = Path.Combine(dstDesktop, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dstFile)!);

                if (!File.Exists(dstFile))
                {
                    File.Copy(srcFile, dstFile, true);
                    added++;
                }
                else
                {
                    var srcTime = File.GetLastWriteTimeUtc(srcFile);
                    var dstTime = File.GetLastWriteTimeUtc(dstFile);
                    if (srcTime > dstTime) { File.Copy(srcFile, dstFile, true); updated++; }
                }
            }
            
            if (DeleteStaleBanks)
            {
                foreach (var dstFile in Directory.GetFiles(dstDesktop, "*.bank", SearchOption.AllDirectories))
                {
                    var rel = Path.GetRelativePath(dstDesktop, dstFile);
                    var srcFile = Path.Combine(srcDesktop, rel);
                    if (!File.Exists(srcFile))
                    {
                        File.Delete(dstFile);
                        var meta = dstFile + ".meta";
                        if (File.Exists(meta)) File.Delete(meta);
                        deleted++;
                    }
                }
            }
            
            if (added + updated > 0)
            {
                foreach (var f in Directory.GetFiles(dstDesktop, "*.bank", SearchOption.AllDirectories))
                {
                    var meta = f + ".meta";
                    if (File.Exists(meta)) File.Delete(meta);
                }
            }

            if (added + updated + deleted > 0) AssetDatabase.Refresh();

            if (VerboseLogs || showDialogs)
                Debug.Log($"[FMOD] Mirror complete → Added: {added}, Updated: {updated}, Deleted: {deleted}. (SRC {srcCount} | DST {dstCount})");
        }

        private static (string srcAbs, string dstAbs, int srcCount, int dstCount) Count()
        {
            var srcAbs = Path.GetFullPath(Path.Combine(Application.dataPath, "..", SrcRoot));
            var dstAbs = Path.GetFullPath(Path.Combine(Application.dataPath, "..", DstRoot));
            int srcCount = Directory.Exists(srcAbs) ? Directory.GetFiles(srcAbs, "*.bank", SearchOption.AllDirectories).Length : 0;
            int dstCount = Directory.Exists(dstAbs) ? Directory.GetFiles(dstAbs, "*.bank", SearchOption.AllDirectories).Length : 0;
            return (srcAbs, dstAbs, srcCount, dstCount);
        }
    }
}
#endif
