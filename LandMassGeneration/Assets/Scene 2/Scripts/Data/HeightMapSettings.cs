using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdateableData
{
    public NoiseSettings noiseSettings;
    
    public bool useFallOff;
    public float heightMultiplier;
    public AnimationCurve heightCurve;

    private void Awake()
    {
        if (noiseSettings.seed == 0)
            noiseSettings.seed = Random.Range(1, 1001);
    }

    public float minHeight
    {
        get => heightMultiplier * heightCurve.Evaluate(0);
    }

    public float maxHeight
    {
        get => heightMultiplier * heightCurve.Evaluate(1);
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}
