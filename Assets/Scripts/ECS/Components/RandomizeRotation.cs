using Unity.Entities;

[GenerateAuthoringComponent]
public struct RandomizeRotation : IComponentData
{
    public float time;
    public float startRate;
    public float finalRate;
    public float rateTime;
    public float lastTime;
}