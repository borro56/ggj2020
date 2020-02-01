using Unity.Entities;

[GenerateAuthoringComponent]
public struct Life : IComponentData
{
    public float amount;
    public float maxAmount;
}