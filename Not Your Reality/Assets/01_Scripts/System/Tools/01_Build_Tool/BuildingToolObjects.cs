#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._04_Tools._01_Build_Tool
{
   [CreateAssetMenu(fileName = "BuildingToolObjects", menuName = "BuildingTool")]
   public class BuildingToolObjects : ScriptableObject
   {
      [Header("Wall Tiles")]
      public List<GameObject> walls = new();
      [Header("Flooring Tiles")]
      public List<GameObject> floors = new();
      [Header("Ceiling Tiles")]
      public List<GameObject> ceilings = new();
      [Header("Small Props")]
      public List<GameObject> propsSmall = new();
      [Header("Medium Props")]
      public List<GameObject> propsMedium = new();
      [Header("Large Props")]
      public List<GameObject> propsLarge = new();
   }
}
#endif