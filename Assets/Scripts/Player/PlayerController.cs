using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private CharacterController character;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera playerVirtualCamera;
    [SerializeField] private List<Behaviour> disableIfNotOwner = new List<Behaviour>();
    [Header("Interact")]
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactRange = 5f;

    [SerializeField] private Animator animator;

    private KinematicCharacterMotor motor;
    private PlayerStats stats;
    private Camera playerCamera;
    private InputManager inputManager;
    private WeaponManager weaponManager;

    public bool OnGround { get => motor.GroundingStatus.IsStableOnGround; }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        name = OwnerClientId.ToString();
        if (!IsOwner)
        {
            Destroy(playerVirtualCamera.gameObject);
            foreach (var item in disableIfNotOwner)
                item.enabled = false;

            weaponManager = GetComponent<WeaponManager>();
            weaponManager.SetupHands();
            return;
        }
        //animator.gameObject.SetActive(false);
        for (int i = 0; i < animator.transform.childCount; i++)
            animator.transform.GetChild(i).gameObject.SetActive(false);
        playerVirtualCamera.transform.parent = null;

        inputManager = InputManager.Instance;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
        weaponManager = GetComponent<WeaponManager>();
        var hands = Camera.main.transform.Find("Hands")?.GetComponent<Hands>();
        hands.GetComponent<SwayAndBob>().SetController(this);
        weaponManager.SetHands(hands);
        weaponManager.SetAttackPoint(playerCamera.transform);
        weaponManager.SetupHands();
        motor = GetComponent<KinematicCharacterMotor>();
        stats = GetComponent<PlayerStats>();
        motor.SetPosition(PlayerSpawn.Instance.transform.position+new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f)));
        motor.SetRotation(PlayerSpawn.Instance.transform.rotation);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        HandleCharacterInput();
        HandleAttacking();
        HandleInteract();

        animator.SetFloat("SpeedX", motor.Velocity.x);
        animator.SetFloat("SpeedZ", -motor.Velocity.z);
    }

    private Interactable lastInteractable = null;

    private void HandleInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactRange))
        {
            var cols = Physics.OverlapSphere(hit.point, 2f, interactableMask);
            float distance = float.MaxValue;
            Interactable closest = null;
            if (cols != null && cols.Length > 0)
            {
                foreach (var item in cols)
                {
                    var inter = item.GetComponent<Interactable>();
                    if (inter != null)
                    {
                        float dist = Vector3.Distance(hit.point, inter.transform.position);
                        if (dist < distance)
                        {
                            distance = dist;
                            closest = inter;
                        }
                    }
                }
            }
            if (closest == null)
            {
                lastInteractable?.OnHoverEnd(this);
            }
            else
            {
                if (lastInteractable != closest)
                {
                    lastInteractable?.OnHoverEnd(this);
                    closest.OnHoverStart(this);
                }
                closest.OnHover(this);
                if (inputManager.PlayerInteractHold)
                {
                    closest.Interact(this, Interactable.InteractionType.Secondary);
                    closest = null;
                }
                if (inputManager.PlayerInteractTrigger)
                {
                    closest.Interact(this, Interactable.InteractionType.Primary);
                    closest = null;
                }
            }
            lastInteractable = closest;
        }
        else
        {
            if (lastInteractable != null)
            {
                lastInteractable?.OnHoverEnd(this);
                lastInteractable = null;
            }
        }
    }

    private void HandleAttacking()
    {
        if (inputManager.PlayerAttackTrigger)
        {
            weaponManager?.Attack();
        }
        if (inputManager.PlayerSwapItemsTrigger)
        {
            if (stats.HasStamina(30))
            {
                weaponManager?.SwapItems();
                stats.TakeStamina(30);
            }
        }
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

        if (characterInputs.JumpDown)
            animator.Play("Jump");

        // Apply inputs to character
        character.SetInputs(ref characterInputs);
    }
}
