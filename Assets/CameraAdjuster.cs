using UnityEngine;
using UnityEngine.XR.Management;

public class CameraOffsetController : MonoBehaviour
{
    [Tooltip("The GameObject that offsets the XR camera up/down")]
    public Transform cameraOffsetObject;

    public float nonXRHeight = 1.36144f; // Camera height when not in XR mode
    public float xrHeightOffset = 0.23856f; // Difference between XR camera height (1.6) and non-XR height (1.36144)

    void Start()
    {
        // Determine the correct offset based on whether XR is active or not
        float yOffset = XRGeneralSettings.Instance.Manager.isInitializationComplete &&
                        XRGeneralSettings.Instance.Manager.activeLoader != null
                        ? xrHeightOffset  // VR mode: adjust for the 1.6 floor height
                        : nonXRHeight;    // Non-VR mode: set the height directly to 1.36144f

        if (cameraOffsetObject != null)
        {
            // Update the camera offset relative to the XR origin
            Vector3 pos = cameraOffsetObject.localPosition;
            cameraOffsetObject.localPosition = new Vector3(pos.x, yOffset, pos.z);
        }
        else
        {
            Debug.LogWarning("Camera Offset Object is not assigned.");
        }
    }

    void Update()
    {
        // Optional: Update XR height dynamically if needed while switching modes
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete &&
            XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            // VR mode handling
            if (cameraOffsetObject != null)
            {
                // Ensure camera offset for VR mode is correct
                cameraOffsetObject.localPosition = new Vector3(cameraOffsetObject.localPosition.x, xrHeightOffset, cameraOffsetObject.localPosition.z);
            }
        }
        else
        {
            // Non-VR mode handling
            if (cameraOffsetObject != null)
            {
                // Set the camera offset for non-VR mode to 1.36144f
                cameraOffsetObject.localPosition = new Vector3(cameraOffsetObject.localPosition.x, nonXRHeight, cameraOffsetObject.localPosition.z);
            }
        }
    }
}
