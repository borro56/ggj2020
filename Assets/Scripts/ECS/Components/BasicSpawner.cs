using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BasicSpawner : IComponentData
{
    public Entity prefab;
    public float startRate;
    public float finalRate;
    public float rateTime;
    public float3 areaSize;
    
    [HideInInspector] public float lastSpawn;
}