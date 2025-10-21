using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Sequence : MonoBehaviour
{
   [Tooltip("This is the order the switches are supposed to be pressed in")]
   [SerializeField] private List<Sequence_Switch> sequenceOrder;
   [SerializeField] private bool toggleDoor;
   [SerializeField] private bool toggleObject;
   private List<Sequence_Switch> _playerOrder;
   private bool _hasWon;
   private Collider _collider;

   private void Awake()
   {
      _playerOrder = new List<Sequence_Switch>();
      if (!toggleDoor) return;
      _collider = GetComponent<Collider>();
      _collider.enabled = false;
   }

   public void AddSwitch(Sequence_Switch sequenceSwitch)
   {
      _playerOrder.Add(sequenceSwitch);
      if (_playerOrder.Count == sequenceOrder.Count && !_hasWon) { StartCoroutine(WinCondition()); }
   }

   private IEnumerator WinCondition()
   {
      yield return new WaitForSeconds(0.2f);
      var counter = 0;
      for (var i = 0; i < _playerOrder.Count; i++)
      {
         if (_playerOrder[i] == sequenceOrder[i])
         {
            counter++;
            continue;
         }

         break;
      }

      if (counter == sequenceOrder.Count)
      {
         Debug.Log("You Win");
         _hasWon = true;
         if (toggleObject) { gameObject.SetActive(false); }

         if (toggleDoor) { _collider.enabled = true; }
      }
      else { Reset(); }
   }

   private void Reset()
   {
      foreach (var seqSwitch in _playerOrder) { seqSwitch.ToggleOff(); }

      Debug.Log("Try Again");
      _playerOrder.Clear();
   }
}