using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    [GenerateAuthoringComponent]
    public struct DamagerDirection : IComponentData
    {
        public float3 Value;
    }
}