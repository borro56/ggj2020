using Unity.Entities;

namespace ECS.Components
{
    [GenerateAuthoringComponent]
    public struct Damager : IComponentData
    {
        public double creationTime;
    }
}