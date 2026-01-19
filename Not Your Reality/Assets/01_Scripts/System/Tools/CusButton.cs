#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace System.Tools
{
   [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
   public class CusButton : Attribute {}

   [CustomEditor(typeof(UnityEngine.Object), true)]
   [CanEditMultipleObjects]
   public class CustomButton : Editor {

      public override void OnInspectorGUI() {
         DrawDefaultInspector();

         MethodInfo[] methods = target.GetType().GetMethods(
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic
         );

         foreach (MethodInfo method in methods) {
            if (method.GetCustomAttribute<CusButton>() == null)
               continue;

            if (GUILayout.Button(method.Name))
               method.Invoke(target, new object[0]);
         }
      }
   }
}
#endif