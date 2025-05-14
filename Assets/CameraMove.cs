using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    [Tooltip("Target the camera rig should move to")]
    public Transform focusTarget;

    [Tooltip("Duration of the transition")]
    public float moveDuration = 1.5f;

    [Tooltip("Degrees to rotate downward when focusing (use negative for downward)")]
    public float focusTiltAngle = -10f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Coroutine transitionCoroutine;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void FocusOnLane()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        Vector3 targetPos = focusTarget.position;

        // Start rotation with current rotation, end with same yaw, but tilted downward
        Quaternion targetRot = Quaternion.Euler(focusTiltAngle, transform.eulerAngles.y, 0f);

        transitionCoroutine = StartCoroutine(MoveToPosition(targetPos, targetRot));
    }

    public void UnfocusFromLane()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(ReturnAfterDelay(3f));
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return MoveToPosition(originalPosition, originalRotation);
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}
