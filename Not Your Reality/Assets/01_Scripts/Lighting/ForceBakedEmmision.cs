using UnityEngine;

namespace _01_Scripts.Lighting
{
   [ExecuteAlways]
   public class ForceBakedEmission : MonoBehaviour
   {
      public Renderer[] targets;
      void OnValidate() => Apply();
      void Apply()
      {
         foreach (var r in targets)
         {
            if (!r) continue;
            foreach (var m in r.sharedMaterials)
            {
               if (!m) continue;
               m.EnableKeyword("_EMISSION");
               m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            }
         }
      }
   }
}