using Unity.Entities;

[GenerateAuthoringComponent]
public struct Attractor : IComponentData
{
    public float Distance;
    public float Force;
    public byte groupId;
}