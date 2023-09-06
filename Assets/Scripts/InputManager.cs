using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    public System.Action<Color> OnCrosshairColorChange;
    public System.Action<float> OnCrosshairSizeChange;
    public System.Action<float> OnMiniMapSizeChange;

    public enum ActionMap
    {
        Movement, Camera, Combat
    }

    [SerializeField] private float bufferTime;
    private Dictionary<InputAction, float> buffer = new Dictionary<InputAction, float>();

    private PlayerInput playerControls;

    public PlayerInput PlayerControls { get => playerControls; }

    public InputActionMap GetActionMap(ActionMap value)
    {
        switch (value)
        {
            case ActionMap.Movement:
                return playerControls.Movement.Get();
            case ActionMap.Camera:
                return playerControls.Camera.Get();
            case ActionMap.Combat:
                return playerControls.Combat.Get();
        }
        return null;
    }

    public InputAction FindAction(string name)
    {
        InputAction action;
        action = playerControls.Movement.Get().FindAction(name);
        if (action != null)
            return action;
        action = playerControls.Camera.Get().FindAction(name);
        if (action != null)
            return action;
        action = playerControls.Combat.Get().FindAction(name);
        if (action != null)
            return action;
        return null;
    }

    private Color crosshairColor;
    private float crosshairSize;
    private float miniMapSize;
    public Color CrosshairColor
    {
        get => crosshairColor; set
        {
            crosshairColor = value;
            OnCrosshairColorChange?.Invoke(value);
        }
    }

    public float CrosshairSize
    {
        get => crosshairSize; set
        {
            crosshairSize = value;
            OnCrosshairSizeChange?.Invoke(value);
        }
    }

    public float MiniMapSize
    {
        get => miniMapSize; set
        {
            miniMapSize = value;
            OnMiniMapSizeChange?.Invoke(value);
        }
    }

    private float GetValueOrDefault(InputAction action, float defaultValue)
    {
        if (buffer.ContainsKey(action))
            return buffer[action];
        return defaultValue;
    }

    public float Sensitivity { get; set; } = 1f;
    public float Scope_Sensitivity { get; set; } = 0.5f;

    public Vector2 PlayerMovement { get { return playerControls.Movement.Movement.ReadValue<Vector2>(); } }
    public Vector2 MouseDelta { get { return playerControls.Camera.Look.ReadValue<Vector2>(); } }
    //public Vector2 MousePosition { get { return playerControls.Additional.MousePosition.ReadValue<Vector2>(); } }
    public bool PlayerJumpedThisFrame { get { return GetValueOrDefault(playerControls.Movement.Jump, -1) > 0; } }
    //public bool PlayerDashTriggered { get { return GetValueOrDefault(playerControls.Movement.Dash, -1) > 0; } }

    //public bool PlayerSprinting { get { return GetValueOrDefault(playerControls.Movement.Sprint, -1) > 0; } }
    public bool PlayerCrouchingHold { get; private set; }

    public bool PlayerAttackTrigger { get { return GetValueOrDefault(playerControls.Combat.Attack, -1) > 0; } }
    public bool PlayerAttackHold { get; private set; }

    private bool playerInteractTrigger;
    private bool playerInteractHold;

    //public bool PlayerInteractTrigger { get => playerInteractTrigger && playerControls.Additional.Interact.triggered; }
    //public bool PlayerInteractHold { get => playerInteractHold && playerControls.Additional.Interact.triggered; }
    //public bool PlayerInventoryTrigger { get => playerControls.Additional.Inventory.triggered; }
    //public bool PlayerMenuTrigger { get => playerControls.Additional.Menu.triggered; }

    //public bool PlayerAbility1 { get => GetValueOrDefault(playerControls.Abilities.Ability1, -1) > 0; }
    //public bool PlayerAbility2 { get => GetValueOrDefault(playerControls.Abilities.Ability2, -1) > 0; }
    //public bool PlayerUltimate { get => GetValueOrDefault(playerControls.Abilities.Ultimate, -1) > 0; }

    //public bool AimDownSightsTrigger { get => GetValueOrDefault(playerControls.Combat.Scope, -1) > 0; }
    //public bool AimDownSightsHold { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
        playerControls = new PlayerInput();

        playerControls.Movement.Crouch.started += (state) => PlayerCrouchingHold = true;
        playerControls.Movement.Crouch.canceled += (state) => PlayerCrouchingHold = false;
        playerControls.Combat.Attack.started += (state) => PlayerAttackHold = true;
        playerControls.Combat.Attack.canceled += (state) => PlayerAttackHold = false;
        //playerControls.Combat.Scope.started += (state) => AimDownSightsHold = true;
        //playerControls.Combat.Scope.canceled += (state) => AimDownSightsHold = false;
        //playerControls.Combat.Reload.started += (state) => PlayerReloadHold = true;
        //playerControls.Combat.Reload.canceled += (state) => PlayerReloadHold = false;
        //playerControls.Additional.Interact.performed += (state) =>
        //{
        //    playerInteractHold = state.interaction is HoldInteraction;
        //    playerInteractTrigger = state.interaction is TapInteraction;
        //};
        //AddToSave(this);
    }

    private void CheckForTriggerInActionMap(InputActionMap map)
    {
        foreach (var action in map.actions)
        {
            if (action.triggered)
            {
                buffer[action] = bufferTime;
            }
        }
    }

    private void Update()
    {
        CheckForTriggerInActionMap(playerControls.Movement.Get());
        CheckForTriggerInActionMap(playerControls.Camera.Get());
        CheckForTriggerInActionMap(playerControls.Combat.Get());

        List<InputAction> removeActions = new List<InputAction>();

        var keys = buffer.Keys.ToList();
        foreach (var item in keys)
        {
            buffer[item] -= Time.deltaTime;
            if (buffer[item] <= 0)
                removeActions.Add(item);
        }

        foreach (var item in removeActions)
            buffer.Remove(item);
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    //private Cinemachine.CinemachinePOV pov;

    //public void SetActiveCamera(CinemachineVirtualCamera cam) => pov = cam?.GetCinemachineComponent<Cinemachine.CinemachinePOV>();
    //private float defaultHorizontalSpeed = -1f;
    //private float defaultVerticalSpeed = -1f;

    //public void ApplySensitivity(bool scoped = false)
    //{
    //    if (pov == null)
    //        return;
    //    if (defaultHorizontalSpeed == -1)
    //        defaultHorizontalSpeed = pov.m_HorizontalAxis.m_MaxSpeed;
    //    if (defaultVerticalSpeed == -1)
    //        defaultVerticalSpeed = pov.m_VerticalAxis.m_MaxSpeed;
    //    pov.m_HorizontalAxis.m_MaxSpeed = defaultHorizontalSpeed * (scoped ? Scope_Sensitivity : Sensitivity);
    //    pov.m_VerticalAxis.m_MaxSpeed = defaultVerticalSpeed * (scoped ? Scope_Sensitivity : Sensitivity);
    //}

    //public void Save()
    //{
    //    var bindingsString = playerControls.asset.SaveBindingOverridesAsJson();
    //    PlayerPrefs.SetString(SaveSystem.BINDINGS, bindingsString);
    //    PlayerPrefs.SetFloat(SaveSystem.SENSITIVITY, Sensitivity);
    //    PlayerPrefs.SetFloat(SaveSystem.SCOPE_SENSITIVITY, Scope_Sensitivity);
    //    PlayerPrefs.SetFloat(SaveSystem.CROSSHAIR_COLOR_RED, crosshairColor.r);
    //    PlayerPrefs.SetFloat(SaveSystem.CROSSHAIR_COLOR_GREEN, crosshairColor.g);
    //    PlayerPrefs.SetFloat(SaveSystem.CROSSHAIR_COLOR_BLUE, crosshairColor.b);
    //    PlayerPrefs.SetFloat(SaveSystem.CROSSHAIR_SIZE, crosshairSize);
    //    PlayerPrefs.SetFloat(SaveSystem.MINIMAP_SIZE, miniMapSize);
    //}

    //public void OnSave(ref Data data)
    //{
    //    Save();
    //}

    //public void OnLoad(Data data)
    //{
    //    playerControls.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString(SaveSystem.BINDINGS));
    //    Sensitivity = PlayerPrefs.GetFloat(SaveSystem.SENSITIVITY);
    //    CrosshairColor = new Color(PlayerPrefs.GetFloat(SaveSystem.CROSSHAIR_COLOR_RED), PlayerPrefs.GetFloat(SaveSystem.CROSSHAIR_COLOR_GREEN), PlayerPrefs.GetFloat(SaveSystem.CROSSHAIR_COLOR_BLUE), 1);
    //    CrosshairSize = PlayerPrefs.GetFloat(SaveSystem.CROSSHAIR_SIZE);
    //    MiniMapSize = PlayerPrefs.GetFloat(SaveSystem.MINIMAP_SIZE);
    //    Scope_Sensitivity = PlayerPrefs.GetFloat(SaveSystem.SCOPE_SENSITIVITY);
    //}
}
