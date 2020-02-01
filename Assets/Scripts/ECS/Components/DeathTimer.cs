using Unity.Entities;

namespace ECS.Components
{
    [GenerateAuthoringComponent]
    public struct DeathTimer : IComponentData
    {
        public float timeWhenDeathTimerStarted;
        public float timeUntilObjectIsDestroyed;
    }
}