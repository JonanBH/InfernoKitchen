using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dish Recipe List", menuName = "InfernoKitchen/New Dish Recipe List")]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList;
}
