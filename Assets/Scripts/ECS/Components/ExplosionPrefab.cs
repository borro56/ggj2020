using Unity.Entities;

[GenerateAuthoringComponent]
public struct ExplosionPrefab : IComponentData
{
    public Entity prefab;
}