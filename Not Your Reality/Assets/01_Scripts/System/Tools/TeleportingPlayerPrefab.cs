using System.Collections.Generic;
using UnityEngine;

namespace System.Tools
{
   public class TeleportingPlayerPrefab : MonoBehaviour
   {
      [SerializeField] private GameObject playerPf;
      [SerializeField] private List<Transform> roomTransforms;

#if UNITY_EDITOR
      [CusButton] public void BasementTp()  => playerPf.transform.position = roomTransforms[0].position;
      [CusButton] public void BedroomTp()   => playerPf.transform.position = roomTransforms[1].position;
      [CusButton] public void DesertTp()    => playerPf.transform.position = roomTransforms[2].position;
      [CusButton] public void BathroomTp()  => playerPf.transform.position = roomTransforms[3].position;
      [CusButton] public void PngTp()       => playerPf.transform.position = roomTransforms[4].position;
      [CusButton] public void BirthdayTp()  => playerPf.transform.position = roomTransforms[5].position;
      [CusButton] public void BalloonTp()   => playerPf.transform.position = roomTransforms[6].position;
      [CusButton] public void TvTp()        => playerPf.transform.position = roomTransforms[7].position;
#endif
   }
}