using ECS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace ECS.Systems
{
    public class HealthBarSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var lifeContainer = GetComponentDataFromEntity<Life>(true);

            return Entities
                .WithReadOnly(lifeContainer)
                .WithAll<HealthBar>()
                .ForEach((ref NonUniformScale scale, in Parent parent) =>
                {
                    var parentLife = lifeContainer[parent.Value];
                    scale.Value.x = parentLife.amount / parentLife.maxAmount;
                }).Schedule(inputDeps);
        }
    }
}