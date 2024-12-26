using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI gamepadKeyInteractText;
    [SerializeField] private TextMeshProUGUI keyAlternateInteractText;
    [SerializeField] private TextMeshProUGUI gamepadKeyAlternateInteractText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField] private TextMeshProUGUI gamepadKeyPauseText;

    private void Start()
    {
        UpdateVisual();
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
        Show();
    }

    private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if(KitchenGameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        gamepadKeyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        keyAlternateInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        gamepadKeyAlternateInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamepadKeyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
