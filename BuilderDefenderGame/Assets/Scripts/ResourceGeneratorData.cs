using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //This is a [Custom Class]. In order for this to show up in the [Inspector] we did [this].
public class ResourceGeneratorData
{
    public float timerMax;

    public ResourceTypeSO resourceType;

    public float resourceDetectionRadius = 5f;

    public int maxResourceAmount;
}
