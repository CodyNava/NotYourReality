using UnityEngine;

namespace PNG_Room
{
    [ExecuteInEditMode]
    public class PngLookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform playerCam;
    
        void Update()
        {
            LookAtPlayer();
        }

        void LookAtPlayer()
        {
            //transform.LookAt(playerCam);
            transform.LookAt(playerCam, Vector3.up);
        }
    }
}
