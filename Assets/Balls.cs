using UnityEngine;

public class BallPickupAndDrag : MonoBehaviour
{
    public float releaseForceZ = -5f;
    public float moveSpeed = 10f;
    public float xMovementForce = 5f;

    private Camera m_currentCamera;
    private Rigidbody rb;
    private Vector3 m_screenPoint;
    private Vector3 m_offset;
    private Vector3 m_currentVelocity;
    private Vector3 m_previousPos;
    private bool isPickedUp = false;
    private bool isThrown = false;

    private bool isInLaneTrigger = false;

    private CameraMove laneCameraFocus; // ✅ Renamed to match your script

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        laneCameraFocus = FindObjectOfType<CameraMove>();
    }

    void Update()
    {
        if (isThrown && !isPickedUp && isInLaneTrigger)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(Vector3.left * xMovementForce);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(Vector3.right * xMovementForce);
            }
        }
    }

    void OnMouseDown()
    {
        m_currentCamera = FindCamera();
        if (m_currentCamera != null)
        {
            m_screenPoint = m_currentCamera.WorldToScreenPoint(transform.position);
            m_offset = transform.position - m_currentCamera.ScreenToWorldPoint(GetMousePosWithScreenZ(m_screenPoint.z));
            rb.useGravity = false;
            isPickedUp = true;
        }
    }

    void OnMouseUp()
    {
        rb.useGravity = true;
        rb.velocity = new Vector3(0, 0, releaseForceZ);
        isPickedUp = false;
        isThrown = true;
        m_currentCamera = null;
    }

    void FixedUpdate()
    {
        if (isPickedUp && m_currentCamera != null)
        {
            Vector3 currentScreenPoint = GetMousePosWithScreenZ(m_screenPoint.z);
            rb.velocity = Vector3.zero;
            rb.MovePosition(m_currentCamera.ScreenToWorldPoint(currentScreenPoint) + m_offset);
            m_currentVelocity = (transform.position - m_previousPos) / Time.deltaTime;
            m_previousPos = transform.position;
        }
    }

    Vector3 GetMousePosWithScreenZ(float screenZ)
    {
        return new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenZ);
    }

    Camera FindCamera()
    {
#if UNITY_2023_1_OR_NEWER
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
#else
        Camera[] cameras = FindObjectsOfType<Camera>();
#endif
        Camera result = null;
        int camerasSum = 0;
        foreach (var camera in cameras)
        {
            if (camera.enabled)
            {
                result = camera;
                camerasSum++;
            }
        }
        return camerasSum == 1 ? result : null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lane"))
        {
            isInLaneTrigger = true;
            if (laneCameraFocus != null)
                laneCameraFocus.FocusOnLane();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lane"))
        {
            isInLaneTrigger = false;
            if (laneCameraFocus != null)
                laneCameraFocus.UnfocusFromLane();
        }
    }
}
