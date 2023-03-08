using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    public event EventHandler<OnActiveBuildingTypeChangeEventArgs> OnActiveBuildingTypeChange;

    [SerializeField] private Building hqBuilding;

    private Camera mainCamera;
    private BuildingTypeListSO buildingTypeList;
    private BuildingTypeSO activeBuildingType;

    public class OnActiveBuildingTypeChangeEventArgs : EventArgs
    {
        public BuildingTypeSO activeBuildingType;
    }

    private void Awake()
    {
        Instance = this;

        //This is to [Load Assets] [in Runtime] [without] having a [direct Refrence]
        //So it will look for a [Folder] [named] ("Resources") in the [Project/Assets]. Continue Down VV
        //And then it's [gonna look] for an [object] that is a [Type Of] [BuildingTypeListSO] And then take his [Name].
        buildingTypeList = Resources.Load<BuildingTypeListSO>(typeof(BuildingTypeListSO).Name);
    }

    private void Start()
    {
        mainCamera = Camera.main;

        hqBuilding.GetComponent<HealthSystem>().OnDied += HQ_OnDied;
    }

    private void HQ_OnDied(object sender, EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.GameOver);

        GameOverUI.Instance.Show();
    }

    private void Update()
    {
        //The [!EventSystem.current.IsPointerOverGameObject()] Means. Continue Down VV
        //It means if the [mouse] is [not above] a [GameObject]
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (activeBuildingType != null)
            {
                if(CanSpawnBuilding(activeBuildingType, UtilsClass.GetMouseWorldPosition(), out string errorMessage))
                {
                    if (ResourceManager.Instance.CanAfford(activeBuildingType.constructionResourceCostArray))
                    {
                        ResourceManager.Instance.SpendResources(activeBuildingType.constructionResourceCostArray);

                        //Instantiate(activeBuildingType.prefab, UtilsClass.GetMouseWorldPosition(), Quaternion.identity);

                        BuildingConstruction.Create(UtilsClass.GetMouseWorldPosition(), activeBuildingType);

                        SoundManager.Instance.PlaySound(SoundManager.Sound.BuildingPlaced);
                    }
                    else
                    {
                        TooltipUI.Instance.Show("Cannot Afford " + activeBuildingType.GetConstructionResourceCostString(), new TooltipUI.TooltipTimer { timer = 2f });
                    }
                }
                else
                {
                    TooltipUI.Instance.Show(errorMessage, new TooltipUI.TooltipTimer { timer = 2f });
                }
            }
        }
    }


    public void SetActiveBuildingType(BuildingTypeSO buildingType)
    {
        activeBuildingType = buildingType;

        OnActiveBuildingTypeChange?.Invoke(this, new OnActiveBuildingTypeChangeEventArgs 
        { 
            activeBuildingType = activeBuildingType 
        });
    }

    public BuildingTypeSO GetActiveBuildingType()
    {
        return activeBuildingType;
    }

    private bool CanSpawnBuilding(BuildingTypeSO buildingType, Vector3 position, out string errorMessage)
    {
        #region Here I'm making sure that the area is Clear to spawn a buildingType
        BoxCollider2D boxCollider2D = buildingType.prefab.GetComponent<BoxCollider2D>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0);

        bool isClear = collider2DArray.Length == 0;

        if (!isClear)
        {
            errorMessage = "Area Is Not Clear!";

            return false;
        }
        #endregion


        #region Here I'm making sure that there are no building of the same Type way to close
        collider2DArray = Physics2D.OverlapCircleAll(position, buildingType.minConsrructionRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            // Colliders inside the Consrruction Radius.
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();

            if (buildingTypeHolder != null)
            {
                // Here I'm making sure the i will not be able to [build] the same [buildingType] next to each other in a given radius.
                // Has a BuildingTypeHolder
                if (buildingTypeHolder.buildingType == buildingType)
                {
                    // Theres already a building of this type within the Consrruction Radius.
                    errorMessage = "Too Close To Another Building Of The Same Type!";

                    return false;
                }
            }
        }
        #endregion

        if (buildingType.hasResourceGeneratorData)
        {
            ResourceGeneratorData resourceGeneratorData = buildingType.resourceGeneratorData;

            int nearbyResourceAmount = ResourceGenerator.GetNearbyResourceAmount(resourceGeneratorData, position);

            if (nearbyResourceAmount == 0)
            {
                errorMessage = "There are no nearby Resource Nodes!";

                return false;
            }
        }


        #region Here I'm making sure that the building is not way to far from any other building
        // Here I'm Making sure that i could not spawn a buildingType really far away.
        float maxConsrructionRadius = 25f;
        collider2DArray = Physics2D.OverlapCircleAll(position, maxConsrructionRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            // Colliders inside the Consrruction Radius.
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();

            if (buildingTypeHolder != null)
            {
                // Its a Building!
                errorMessage = "";

                return true;
            }
        }
        #endregion

        errorMessage = "Too Far From Any Other Building!";

        return false;
    }

    public Building GetHQBuilding()
    {
        return hqBuilding;
    }
}
