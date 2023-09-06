using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private CharacterController character;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerVirtualCamera;
    [SerializeField] private List<Behaviour> disableIfNotOwner = new List<Behaviour>();

    private Camera playerCamera;
    private InputManager inputManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            Destroy(playerVirtualCamera.gameObject);
            foreach (var item in disableIfNotOwner)
                item.enabled = false;
        }
        else
            playerVirtualCamera.transform.parent = null;
    }

    private void Start()
    {
        if (!IsOwner) {
            return;
        }
        inputManager = InputManager.Instance;

        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;

    }

    private void Update()
    {
        if (IsOwner)
            HandleCharacterInput();
    }


    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = inputManager.PlayerMovement.y;
        characterInputs.MoveAxisRight = inputManager.PlayerMovement.x;
        characterInputs.CameraRotation = playerCamera.transform.rotation;
        characterInputs.JumpDown = inputManager.PlayerJumpedThisFrame;
        characterInputs.CrouchDown = inputManager.PlayerCrouchingHold;
        characterInputs.CrouchUp = !inputManager.PlayerCrouchingHold;

        // Apply inputs to character
        character.SetInputs(ref characterInputs);
    }
}
