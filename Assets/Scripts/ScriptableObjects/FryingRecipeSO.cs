using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Frying Recipe", menuName = "InfernoKitchen/New Frying Recipe")]
public class FryingRecipeSO : ScriptableObject
{

    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax = 3;
}
