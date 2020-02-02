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
            var time = UnityEngine.Time.realtimeSinceStartup;
            var deltaTime = Time.DeltaTime;
            var job = Entities
                .ForEach((Entity entity, ref DeathTimer deathTimer) =>
                {
                    deathTimer.timeUntilObjectIsDestroyed -= deltaTime;
                    //if (time - deathTimer.timeWhenDeathTimerStarted > deathTimer.timeUntilObjectIsDestroyed)
                    if(deathTimer.timeUntilObjectIsDestroyed <= 0)
                    {
                        commandBuffer.DestroyEntity(0, entity);
                    }
                }).Schedule(inputDeps);
            buffer.AddJobHandleForProducer(job);
            
            return job;
        }
    }
}