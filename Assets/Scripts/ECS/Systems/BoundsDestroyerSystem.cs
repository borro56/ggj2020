using ECS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class BoundsDestroyerSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem buffer;

        protected override void OnCreate()
        {
            buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var bounds = new Bounds();
            bounds.min = DeathBoundsGlobal.Instance.min;
            bounds.max = DeathBoundsGlobal.Instance.max;
            
            var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();
            var job = Entities
                .ForEach((Entity entity, in DeathBounds deathTimer, in Translation trans) =>
                {
                    if (!bounds.Contains(trans.Value))
                        commandBuffer.DestroyEntity(0, entity);
                }).Schedule(inputDeps);
            buffer.AddJobHandleForProducer(job);
            
            return job;
        }
    }
}