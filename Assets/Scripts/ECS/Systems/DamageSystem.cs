using ECS.Components;
using Globals;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

namespace ECS.Systems
{
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class DamageSystem : JobComponentSystem
    {
        private EntityQuery _dangerEntities;
        private EndSimulationEntityCommandBufferSystem _buffer;

        protected override void OnCreate()
        {
            _buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _dangerEntities = GetEntityQuery(
                ComponentType.ReadOnly<Damager>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Team>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            
            var damage = DamagerPropertiesGlobal.Instance.damage;
            var commandBuffer = _buffer.CreateCommandBuffer().ToConcurrent();
            var dangerEntities = _dangerEntities.ToEntityArray(Allocator.TempJob);
            var dangerPositions = _dangerEntities.ToComponentDataArray<Translation>(Allocator.TempJob);
            var dangerTeam = _dangerEntities.ToComponentDataArray<Team>(Allocator.TempJob);
            
            var job = Entities
                .WithDeallocateOnJobCompletion(dangerEntities)
                .WithDeallocateOnJobCompletion(dangerPositions)
                .WithDeallocateOnJobCompletion(dangerTeam)
                .ForEach((Entity e, ref Life life, in WorldRenderBounds bounds, in Team team) =>
                {
                    float accumulatedDamage = 0;
                    for (var i = 0; i < dangerPositions.Length; i++)
                    {
                        if(i < dangerTeam.Length && dangerTeam[i].id == team.id) continue;

                        var position = dangerPositions[i].Value;
                        if (bounds.Value.Contains(position))
                        {
                            accumulatedDamage += damage;
                            
                            commandBuffer.DestroyEntity(0, dangerEntities[i]);
                        }
                    }

                    if (accumulatedDamage > 0)
                    {
                        commandBuffer.SetComponent(0, e, new Life
                        {
                            amount =  math.max(life.amount - accumulatedDamage, 0),
                            maxAmount = life.maxAmount
                        });
                    }
                }).Schedule(inputDeps);
            
            _buffer.AddJobHandleForProducer(job);
            return job;
        }
    }
}