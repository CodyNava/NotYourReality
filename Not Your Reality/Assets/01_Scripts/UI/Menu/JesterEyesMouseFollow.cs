using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    
    //[ExecuteInEditMode]
    public class JesterEyesMouseFollow : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        
        void Update()
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mouse);
            
            Plane plane = new Plane(-cam.transform.forward, transform.position);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 point = ray.GetPoint(enter/ 2);
                transform.LookAt(point );
            }
        }
    }
}
