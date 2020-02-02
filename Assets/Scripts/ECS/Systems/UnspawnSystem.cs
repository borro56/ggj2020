using ECS.Components;
using Globals;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class UnspawnSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem buffer;

        protected override void OnCreate()
        {
            buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();
            var time = UnityEngine.Time.timeSinceLevelLoad;
            var job = Entities
                .ForEach((Entity entity, in DeathTimer deathTimer) =>
                {
                    if (time - deathTimer.timeWhenDeathTimerStarted > deathTimer.timeUntilObjectIsDestroyed)
                    {
                        commandBuffer.DestroyEntity(0, entity);
                    }
                }).Schedule(inputDeps);
            buffer.AddJobHandleForProducer(job);
            
            return job;
        }
    }
}