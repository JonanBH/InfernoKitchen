using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private const string PLAYER_PREFS_BINDINGS = "Bindings";

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause
    }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += InteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;

        if(PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= InteractPerformed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector.normalized;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.MoveUp:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();

            case Binding.MoveDown:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();

            case Binding.MoveLeft:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();

            case Binding.MoveRight:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();

            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();

            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();

            default:
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();

        }
    }

    public void RebindBinding(Binding binding, Action onActionReboundCallback)
    {
        playerInputActions.Disable();

        InputAction inputAction = playerInputActions.Player.Move; ;
        int rebindingIndex = 1;
        switch (binding)
        {
            case Binding.MoveUp:
                inputAction = playerInputActions.Player.Move;
                rebindingIndex = 1;
                break;

            case Binding.MoveDown:
                inputAction = playerInputActions.Player.Move;
                rebindingIndex = 2;
                break;

            case Binding.MoveLeft:
                inputAction = playerInputActions.Player.Move;
                rebindingIndex = 3;
                break;

            case Binding.MoveRight:
                inputAction = playerInputActions.Player.Move;
                rebindingIndex = 4;
                break;

            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                rebindingIndex = 0;
                break;

            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                rebindingIndex = 0;
                break;

            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                rebindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(rebindingIndex).OnComplete(callback => 
        {
            callback.Dispose();

            playerInputActions.Player.Enable();
            onActionReboundCallback?.Invoke();

            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }).Start() ;
    }
}
