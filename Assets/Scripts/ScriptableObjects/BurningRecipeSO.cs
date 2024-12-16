using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Burning Recipe", menuName = "InfernoKitchen/New Burning Recipe")]
public class BurningRecipeSO : ScriptableObject
{

    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax = 3;
}
