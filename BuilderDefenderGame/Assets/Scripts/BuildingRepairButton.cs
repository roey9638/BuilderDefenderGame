using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingRepairButton : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private ResourceTypeSO goldResourceType;

    private void Awake()
    {
        transform.Find("button").GetComponent<Button>().onClick.AddListener(() =>
        {
            int missingHealth = healthSystem.GetHealthAmountMax() - healthSystem.GetHealthAmount();

            int repairCost = missingHealth / 2;

            ResourceAmount[] resourceAmountCost = new ResourceAmount[] {
                new ResourceAmount { resourceType = goldResourceType, amount = repairCost } };

            if (ResourceManager.Instance.CanAfford(resourceAmountCost))
            {
                // Cann Afford the Repairs
                ResourceManager.Instance.SpendResources(resourceAmountCost);

                healthSystem.HealFull();
            }
            else
            {
                // Cannnot afford the repairs!
                TooltipUI.Instance.Show("Cannnot afford repairs cost!", new TooltipUI.TooltipTimer { timer = 2f });
            }

        });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
