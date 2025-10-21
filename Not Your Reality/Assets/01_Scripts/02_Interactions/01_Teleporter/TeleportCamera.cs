using UnityEngine;

public class PortalView : MonoBehaviour
{
    [Space]
    public Transform entryPortal;
    public Transform exitPortal;
    public Camera portalCamera;
    public Camera playerCamera;

    private void LateUpdate()
    {
        if (!entryPortal || !exitPortal || !portalCamera || !playerCamera) return;
        
        var relativePos = entryPortal.InverseTransformPoint(playerCamera.transform.position);
        var relativeRot = Quaternion.Inverse(entryPortal.rotation) * playerCamera.transform.rotation;

        portalCamera.transform.position = exitPortal.TransformPoint(relativePos);
        portalCamera.transform.rotation = exitPortal.rotation * relativeRot;
        
    }
}