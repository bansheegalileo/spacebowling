using UnityEngine;
using UnityEngine.XR;

public class SmoothFOVAdjuster : MonoBehaviour
{
    public Camera targetCamera;
    public Transform cameraPivot; // Assign in Inspector (parent of camera)

    public float fovStep = 10f;
    public float minFOV = 30f;
    public float maxFOV = 90f;
    public float smoothSpeed = 5f;

    public float panSensitivity = 100f;
    public float vrPanSpeed = 45f;

    private float targetFOV;
    private bool inputLocked = false;
    private float lastMouseX;
    private float currentYRotation = 0f;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (cameraPivot == null)
            Debug.LogError("Camera pivot must be assigned.");

        targetFOV = targetCamera.fieldOfView;
        lastMouseX = Input.mousePosition.x;

        currentYRotation = 180f;
        cameraPivot.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }

    void Update()
    {
        HandleFOVInput();
        SmoothFOVTransition();

        if (XRSettings.isDeviceActive)
            HandleVRPan();
        else
            HandleMousePan();
    }

    void HandleFOVInput()
    {
        float yInput = 0f;

        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primaryAxis))
        {
            yInput = primaryAxis.y;

            if (!inputLocked)
            {
                if (yInput < -0.5f) AdjustFOV(fovStep);
                else if (yInput > 0.5f) AdjustFOV(-fovStep);

                if (Mathf.Abs(yInput) > 0.5f)
                {
                    inputLocked = true;
                    Invoke(nameof(ResetInputLock), 0.3f);
                }
            }
        }

        if (!XRSettings.isDeviceActive)
        {
            if (Input.GetKeyDown(KeyCode.S)) AdjustFOV(fovStep);
            else if (Input.GetKeyDown(KeyCode.W)) AdjustFOV(-fovStep);
        }
    }

    void AdjustFOV(float amount)
    {
        targetFOV = Mathf.Clamp(targetFOV + amount, minFOV, maxFOV);
    }

    void SmoothFOVTransition()
    {
        if (Mathf.Abs(targetCamera.fieldOfView - targetFOV) > 0.01f)
        {
            targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
        }
    }

    void HandleMousePan()
    {
        float currentMouseX = Input.mousePosition.x;
        float deltaX = currentMouseX - lastMouseX;

        currentYRotation += deltaX * panSensitivity * Time.deltaTime;
        cameraPivot.rotation = Quaternion.Euler(0f, currentYRotation, 0f);

        lastMouseX = currentMouseX;
    }

    void HandleVRPan()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            float horizontalInput = axis.x;
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                currentYRotation += horizontalInput * vrPanSpeed * Time.deltaTime;
                cameraPivot.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
            }
        }
    }

    void ResetInputLock()
    {
        inputLocked = false;
    }
}
