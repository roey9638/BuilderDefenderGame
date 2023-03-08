using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private float secondsPerDay = 10f;
    private Light2D light2d;
    private float dayTime;
    private float dayTimeSpeed;

    private void Awake()
    {
        light2d = GetComponent<Light2D>();

        dayTimeSpeed = 1 / secondsPerDay;
    }

    private void Update()
    {
        dayTime += Time.deltaTime * dayTimeSpeed;

        // The [dayTime % 1f] is for when [it's passes] (1f) it will [Reset back to] (0f).
        light2d.color = gradient.Evaluate(dayTime % 1f);
    }
}
