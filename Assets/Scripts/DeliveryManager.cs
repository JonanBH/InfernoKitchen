using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeSuccess;

    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private int waitingRecipesMax = 4;

    public static DeliveryManager Instance { get; private set; }

    private int successfulRecipesAmount = 0;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 10;


    private void Awake()
    {
        waitingRecipeSOList = new List<RecipeSO>();
        spawnRecipeTimer = spawnRecipeTimerMax;
        Instance = this;
    }


    private void Update()
    {
        if(!IsServer) return;

        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax && KitchenGameManager.Instance.IsGamePlaying())
            {
                int selectedRecipeId = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRPC(selectedRecipeId);
                
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRPC(int waitingRecipeSOIndex)
    {
        waitingRecipeSOList.Add(recipeListSO.recipeSOList[waitingRecipeSOIndex]);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }


    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) 
    {
        for(int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO plateRecipeKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if(plateRecipeKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                        break;
                    }
                }

                if(plateContentsMatchesRecipe)
                {
                    // Correct recipe delivered
                    Debug.Log("Player delivered the correct recipe!");
                    DeliverCorrectRecipeServerRpc(i);

                    return;
                }
            }
        }

        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeIndex)
    {
        waitingRecipeSOList.RemoveAt(waitingRecipeIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        successfulRecipesAmount++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        Debug.Log("Player did not delivered a correct recipe");
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}
