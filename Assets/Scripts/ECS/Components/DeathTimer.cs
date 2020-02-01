using Unity.Entities;

namespace ECS.Components
{
    public struct DeathTimer : IComponentData
    {
        public float timeWhenDeathTimerStarted;
        public float timeUntilObjectIsDestroyed;
    }
}