using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChromaticAberrationEffect : MonoBehaviour
{
    public static ChromaticAberrationEffect Instance { get; private set; }

    private Volume volume;

    private void Awake()
    {
        Instance = this;

        volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (volume.weight > 0)
        {
            float decreaseSpeed = 1f;

            volume.weight -= Time.deltaTime * decreaseSpeed;
        }
    }

    public void SetWeigth(float weight)
    {
        volume.weight = weight;
    }
}
