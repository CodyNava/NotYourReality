using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

namespace Puzzle.Desert_Reflection_Room
{
   
   public class MirrorLightReflection : MonoBehaviour
   {
      [Tooltip("The light source of this riddle")]
      [SerializeField] private LightSource lightSource;

      [Tooltip("The goal")]
      [SerializeField] private ReflectionGoal reflectionGoal;

      [Tooltip("The material the Goal should have when it is hit by the Light")]
      [SerializeField] private Material targetWinMaterial;

      [Tooltip("The material the Goal should have when it is not hit by the Light")]
      [SerializeField] private Material targetDefaultMaterial;

      [Tooltip("The door that is supposed to be activated once the Goal is hit")]
      [SerializeField] private DoorHandle door ;

      private bool _puzzleCompleted;
      private readonly List<GameObject> _objectsToReset = new();
      private readonly List<Vector3> _initialPosition = new();
      private readonly List<Quaternion> _initialRotation = new();
      private readonly List<Renderer> _targetRenderer = new();
      public List<ReflectionGoal> goals = new();

      private void Awake()
      {

         foreach (Transform child in transform)
         {
            _initialPosition.Add(child.position);
            _initialRotation.Add(child.rotation);
            _objectsToReset.Add(child.gameObject);
            if (!child.CompareTag("Goal")) continue;
            goals.Add(child.gameObject.GetComponent<ReflectionGoal>());
            _targetRenderer.Add(child.gameObject.GetComponent<Renderer>());
         }
         foreach (var target in _targetRenderer) { target.material = targetDefaultMaterial; }

         if (!door) return;
         door.IsInteractable = false;
      }
      

      public void CheckWin()
      {
         for (int i = 0; i < goals.Count; i++)
         {
            _targetRenderer[i].material = goals[i].BeenHit()? targetWinMaterial: targetDefaultMaterial;
         }

         if (!door) return;

         //todo: more feedback for winning

         if (!AllHit() || _puzzleCompleted) return;
         _puzzleCompleted = true;
         door.IsInteractable = true;
      }

      private bool AllHit()
      {
         var winCounter = goals.Count(goal => goal.BeenHit());
         return winCounter == goals.Count;
      }

      public IEnumerator ResetRiddle()
      {
         for (var i = 0; i < _objectsToReset.Count; i++)
         {
            _objectsToReset[i].transform.position = _initialPosition[i];
            _objectsToReset[i].transform.rotation = _initialRotation[i];
            yield return null;
         }

         reflectionGoal.ClearHits();
      }
   }
}