using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    PlayerResource playerResource;

    public bool isInteracting;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindFirstObjectByType<CameraManager>();
        playerResource = GetComponent<PlayerResource>();
    }

    private void Update()
    {

        if (playerResource.IsAlive)
        {
            playerResource.HandleDrain(playerLocomotion.isSprinting);
        }
        else
        {
            isInteracting = true;
            return;
        }

        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
    }
}