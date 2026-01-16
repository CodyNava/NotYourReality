using System.Collections;
using UnityEngine;

namespace Interactions.Teleporter
{
   public class TeleportTransformSpawn : MonoBehaviour
   {
      [SerializeField] private float speed;
      [SerializeField] private Vector3 offset;
      private bool _spawnTriggered;
      private bool _moved;

      public IEnumerator MoveToDestination(bool start)
      {
         var t = 0f;
         offset = start ? offset : -offset;
         while (t < speed)
         {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, transform.position -= offset, speed * Time.deltaTime);
            yield return null;
         }
      }
   }
}