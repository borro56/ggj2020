using Unity.Entities;

[GenerateAuthoringComponent]
public struct Healer : IComponentData
{
    public Entity target;
}