using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform;  // object camera follow
    public Transform cameraPivot;      // object camera to use to pivot (look up/down)
    public Transform cameraTransform;  // transform of the actual object in scene
    public LayerMask collisionsLayers; // layer camera collision 

    private float currentTargetZoom_Z;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffSet = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;

    public float lookAngle;  // up, down
    public float pivotAngle; // left, right
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;

    [Header("Camera Zoom")]
    [SerializeField] private float zoomStepAmount = 1.0f;
    [SerializeField] private float zoomSmoothSpeed = 10f;
    [SerializeField] private float minZoomDistance = 2f;
    [SerializeField] private float maxZoomDistance = 10f;

    [Header("Camera Smoothing")]
    [SerializeField] private float rotationSmoothSpeed = 8f;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        targetTransform = FindFirstObjectByType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        currentTargetZoom_Z = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleZoom();
        HandleCameraCollisions();
    }

    private void HandleZoom()
    {
        float scrollInput = 0f;
        if (Mouse.current != null)
        {
            scrollInput = Mathf.Sign(Mouse.current.scroll.y.ReadValue());
        }

        if (scrollInput != 0)
        {
            currentTargetZoom_Z += scrollInput * zoomStepAmount;
        }

        currentTargetZoom_Z = Mathf.Clamp(currentTargetZoom_Z, -maxZoomDistance, -minZoomDistance);
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Quaternion targetHorizontalRotation = Quaternion.Euler(0, lookAngle, 0);
        Quaternion targetVerticalRotation = Quaternion.Euler(pivotAngle, 0, 0);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetHorizontalRotation,
            rotationSmoothSpeed * Time.deltaTime);

        cameraPivot.localRotation = Quaternion.Slerp(
            cameraPivot.localRotation,
            targetVerticalRotation,
            rotationSmoothSpeed * Time.deltaTime);
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = currentTargetZoom_Z;

        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionsLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }

        Vector3 newLocalPosition = cameraTransform.localPosition;

        newLocalPosition.z = Mathf.Lerp(newLocalPosition.z, targetPosition, zoomSmoothSpeed * Time.deltaTime);

        cameraTransform.localPosition = newLocalPosition;

    }
}