using Unity.Entities;

[GenerateAuthoringComponent]
public struct Attractor : IComponentData
{
    public byte groupId;
}