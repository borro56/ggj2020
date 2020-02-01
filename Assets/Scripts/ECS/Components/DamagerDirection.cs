using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    //TODO: Replace with PhysicsVelocity
    [GenerateAuthoringComponent]
    public struct DamagerDirection : IComponentData
    {
        public float3 Value;
    }
}