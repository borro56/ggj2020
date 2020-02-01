using Unity.Entities;

[GenerateAuthoringComponent]
public struct RandomizeRotation : IComponentData
{
    public float rate;
    public float lastTime;
}