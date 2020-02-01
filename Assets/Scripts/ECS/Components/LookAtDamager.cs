using Unity.Entities;
using Unity.Mathematics;

public struct LookAtDamager : IComponentData
{
    public quaternion initialRotation;
}