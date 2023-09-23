using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayAndBob : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerController controller;

    [Header("Sway")]
    [SerializeField] private bool sway = true;
    [SerializeField] private bool swayRotation = true;
    [SerializeField] private bool bobOffset = true;
    [SerializeField] private bool bobSway = true;
    [SerializeField] private float step = 0.01f;
    [SerializeField] private float maxStepDistance = 0.06f;
    [SerializeField] private float rotationStep = 4f;
    [SerializeField] private float maxRotationStep = 5f;
    [SerializeField] private float smooth = 10f; 

    [Header("Bobbing")]
    [SerializeField] private float speedCurve;
    [SerializeField] private Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 bobLimit = Vector3.one * 0.01f;
    [SerializeField] private Vector3 multiplier;

    private float curveSin { get => Mathf.Sin(speedCurve); }
    private float curveCos { get => Mathf.Cos(speedCurve); }


    private Vector3 bobPosition;
    private Vector3 swayPos;
    private Vector3 swayEulerRot;
    private Vector3 bobEulerRotation;
    private Vector3 weaponShakePosition;
    private Vector3 weaponShakeRotation;

    InputManager inputManager;
    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void SetController(PlayerController controller)
    {
        this.controller = controller;
        this.rb = controller.GetComponent<Rigidbody>();
    }

    private void Sway()
    {
        if (!sway)
        {
            swayPos = Vector3.zero;
            return;
        }
        Vector3 invertLook = inputManager.MouseDelta * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }
    private void SwayRotation()
    {
        if (!swayRotation)
        {
            swayEulerRot = Vector3.zero;
            return;
        }
        Vector3 invertLook = inputManager.MouseDelta * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    private void BobOffset()
    {
        speedCurve += Time.deltaTime * (controller.OnGround ? rb.velocity.magnitude : 1f) + 0.01f;

        if (!bobOffset)
        {
            bobPosition = Vector3.zero;
            return;
        }

        bobPosition.x = (curveCos * bobLimit.x * (controller.OnGround ? 1 : 0)) - (inputManager.PlayerMovement.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y ) - (rb.velocity.y * travelLimit.y);
        bobPosition.z = - (inputManager.PlayerMovement.y * travelLimit.z);
    }

    private void BobRotation()
    {
        if (!bobSway)
        {
            bobEulerRotation = Vector3.zero;
            return;
        }

        bobEulerRotation.x = (inputManager.PlayerMovement != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2*speedCurve) / 2));
        bobEulerRotation.y = (inputManager.PlayerMovement != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (inputManager.PlayerMovement != Vector2.zero ? multiplier.z * curveCos * inputManager.PlayerMovement.x : 0);
    }

    private void Update()
    {
        if (controller == null)
            return;
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionAndRotation();
    }


    private void CompositePositionAndRotation()
    {

        transform.localPosition = Vector3.Lerp(transform.localPosition, (swayPos + bobPosition) + weaponShakePosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot)
               * Quaternion.Euler(bobEulerRotation)
               * Quaternion.Euler(weaponShakeRotation), Time.deltaTime * smooth);

       /* Vector3 maxWeaponShakeRotation = playerWeaponManager.Weapon.weaponShakeRotation;
        Vector3 maxWeaponShakePosition = playerWeaponManager.Weapon.weaponShakePosition;
        float shootCooldown = Weapon.GetShootTime(playerWeaponManager.Weapon);

        weaponShakeRotation.x = Mathf.Lerp(weaponShakeRotation.x, weaponShakeRotation.x + shootCooldown * maxWeaponShakeRotation.x, 0.1f);
        weaponShakeRotation.y = Mathf.Lerp(weaponShakeRotation.y, weaponShakeRotation.y - shootCooldown * maxWeaponShakeRotation.y, 0.1f);
        weaponShakeRotation.z = Mathf.Lerp(weaponShakeRotation.z, weaponShakeRotation.z - shootCooldown * maxWeaponShakeRotation.z, 0.1f);

        weaponShakeRotation.x = Mathf.Clamp(weaponShakeRotation.x, -maxWeaponShakeRotation.x, 0);
        weaponShakeRotation.y = Mathf.Clamp(weaponShakeRotation.y, 0, maxWeaponShakeRotation.y);
        weaponShakeRotation.z = Mathf.Clamp(weaponShakeRotation.z, 0, maxWeaponShakeRotation.z);

        weaponShakePosition.x = Mathf.Lerp(weaponShakePosition.x, weaponShakePosition.x - shootCooldown * maxWeaponShakePosition.x, 0.1f);
        weaponShakePosition.y = Mathf.Lerp(weaponShakePosition.y, weaponShakePosition.y - shootCooldown * maxWeaponShakePosition.y, 0.1f);
        weaponShakePosition.z = Mathf.Lerp(weaponShakePosition.z, weaponShakePosition.z - shootCooldown * maxWeaponShakePosition.z, 0.1f);

        weaponShakePosition.x = Mathf.Clamp(weaponShakePosition.x, 0, maxWeaponShakePosition.x);
        weaponShakePosition.y = Mathf.Clamp(weaponShakePosition.y, 0, maxWeaponShakePosition.y);
        weaponShakePosition.z = Mathf.Clamp(weaponShakePosition.z, 0, maxWeaponShakePosition.z);*/

    }
}
