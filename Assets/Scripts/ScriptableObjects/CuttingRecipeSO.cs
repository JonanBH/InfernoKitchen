using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutting Recipe", menuName = "InfernoKitchen/New Cutting Recipe")]
public class CuttingRecipeSO : ScriptableObject
{

    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax = 3;
}
