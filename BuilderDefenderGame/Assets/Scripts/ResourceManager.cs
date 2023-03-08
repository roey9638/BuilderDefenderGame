using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public event EventHandler OnResourceAmountChange;

    [SerializeField] private List<ResourceAmount> startingResourceAmountList;
    //1) In [Dictionary<>] you can [Define] a [Type for the "Key"] and a [Type for the "Value"].
    //2) You can [Access] the [Value] by [Using] the [Key].
    //3) In this case, The [Key is ("ResourceTypeSO") ]. And the [Value is ("int") ].
    private Dictionary<ResourceTypeSO, int> resourceAmountDictionary;

    private void Awake()
    {
        Instance = this;

        resourceAmountDictionary = new Dictionary<ResourceTypeSO, int>();

        ResourceTypeListSO resourceTypeList = Resources.Load<ResourceTypeListSO>(typeof(ResourceTypeListSO).Name);

        //1) Here I'm going threw the [list] which is in the [Class "ResourceTypeListSO"]
        foreach (ResourceTypeSO resourceType in resourceTypeList.list)
        {
            //Here I'm [initializing] my [resourceAmountDictionary] [for each] [resourceType] to start at (0)
            resourceAmountDictionary[resourceType] = 0;
        }


        foreach (ResourceAmount resourceAmount in startingResourceAmountList)
        {
            AddResource(resourceAmount.resourceType, resourceAmount.amount);
        }
    }

    private void TestLogResourceAmountDictionary()
    {
        foreach (ResourceTypeSO resourceType in resourceAmountDictionary.Keys)
        {
            Debug.Log(resourceType.nameString + ": " + resourceAmountDictionary[resourceType]);
        }
    }

    public void AddResource(ResourceTypeSO resourceType, int amount)
    {
        resourceAmountDictionary[resourceType] += amount;

        //This is an [Event]. So [Something Changes] on this [Function] it will [Trigger] this [Event].
        //The [?.Invoke] [Checks] if it's [Not Null]. That means [Someone] is [listening] to this [Event]. Continue Down VV
        //then [Invoke/Trigger] this [Event].
        //We [Listen] to the [Event] in the [ResourceUI] [Script] [Under] the [Start Function].
        OnResourceAmountChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetResourceAmount(ResourceTypeSO resourceType)
    {
        return resourceAmountDictionary[resourceType];
    }

    public bool CanAfford(ResourceAmount[] resourceAmountArray)
    {
        foreach (ResourceAmount resourceAmount in resourceAmountArray)
        {
            if (GetResourceAmount(resourceAmount.resourceType) >= resourceAmount.amount)
            {
                // Can Afford
            }
            else
            {
                // Cannot Afford this!
                return false;
            }
        }

        // Can Afford all
        return true;
    }

    public void SpendResources(ResourceAmount[] resourceAmountArray)
    {
        foreach (ResourceAmount resourceAmount in resourceAmountArray)
        {
            resourceAmountDictionary[resourceAmount.resourceType] -= resourceAmount.amount;         
        }
    }
}
