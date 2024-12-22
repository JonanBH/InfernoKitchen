using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Kitchen Object SO List", menuName = "InfernoKitchen/New Kitchen Object SO List")]
public class KitchenObjectListSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
}
