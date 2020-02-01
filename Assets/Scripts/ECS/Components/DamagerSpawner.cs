using Unity.Entities;

namespace ECS.Components
{
    public struct DamagerSpawner : IComponentData
    {
        public Entity prefab;
        public float radius;
        public double lastCreatedTime;
    }
}