using Unity.Entities;

[GenerateAuthoringComponent]
public struct BasicSpawner : IComponentData
{
    public Entity prefab;
    public float rate;
    public float lastSpawn;
}