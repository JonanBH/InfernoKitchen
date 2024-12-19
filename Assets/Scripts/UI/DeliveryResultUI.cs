using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{

    const string POPUP = "Popup";

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Color successColor;
    [SerializeField] private Sprite successSprite;

    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite failedSprite;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        ShowPopup(failedColor, failedSprite, "DELIVERY\nFAILED");
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        ShowPopup(successColor, successSprite, "DELIVERY\nSUCCESS");
    }

    private void ShowPopup(Color color, Sprite icon, string message )
    {
        gameObject.SetActive(true);
        backgroundImage.color = color;
        iconImage.sprite = icon;
        messageText.text = message;

        animator.SetTrigger(POPUP);
    }
}
