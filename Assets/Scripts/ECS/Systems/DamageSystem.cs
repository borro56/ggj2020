using ECS.Components;
using Globals;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace ECS.Systems
{
    [UpdateAfter(typeof(HealingSystem))]
    public class DamageSystem : JobComponentSystem
    {
        private EntityQuery _dangerEntities;
        private EndSimulationEntityCommandBufferSystem _buffer;

        protected override void OnCreate()
        {
            _buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _dangerEntities = GetEntityQuery(
                ComponentType.ReadOnly<Damager>()
                , ComponentType.ReadOnly<Translation>()
                , ComponentType.ReadOnly<LocalToWorld>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            
            var damage = DamagerPropertiesGlobal.Instance.damage;
            var damageDistance = DamagerPropertiesGlobal.Instance.damageDistance;
            var commandBuffer = _buffer.CreateCommandBuffer().ToConcurrent();
            var dangerEntities = _dangerEntities.ToEntityArray(Allocator.TempJob);
            var dangerPositions = _dangerEntities.ToComponentDataArray<Translation>(Allocator.TempJob);
            var dangerL2W = _dangerEntities.ToComponentDataArray<Translation>(Allocator.TempJob);
            
            var job = Entities
                .WithDeallocateOnJobCompletion(dangerEntities)
                .WithDeallocateOnJobCompletion(dangerPositions)
                .WithDeallocateOnJobCompletion(dangerL2W)
                .ForEach((Entity e, ref Life life, in WorldRenderBounds bounds) =>
                {
                    float accumulatedDamage = 0;
                    for (var i = 0; i < dangerPositions.Length; i++)
                    {
                        var position = dangerPositions[i];
                        var pos = math.mul(new float4(position.Value, 1), dangerL2W[i].Value).xyz;
                        var flattenCenter = bounds.Value.Center;
                        
                        flattenCenter.y = 0;
                        pos.y = 0;
                        
                        var sqdist = math.distancesq(pos, flattenCenter);

                        if (sqdist < damageDistance)
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