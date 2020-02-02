using Unity.Entities;

[GenerateAuthoringComponent]
public struct Rotator : IComponentData
{
    public float velocity;
}