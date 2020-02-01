using ECS.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


[UpdateBefore(typeof(DamageSystem))]
public class HealingSystem : JobComponentSystem
{
    EntityQuery targetEntities;
    private EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        targetEntities = GetEntityQuery(ComponentType.ReadOnly<Healer>(), ComponentType.ReadOnly<LocalToWorld>(),  ComponentType.ReadOnly<Translation>());
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();

        var healDistance = HealerGlobal.Instance.Distance;
        var healAmmount = HealerGlobal.Instance.Amount;
        var deltaTime = Time.DeltaTime;
        // get entity references
        var healerPosition = targetEntities.ToComponentDataArray<Translation>(Allocator.TempJob);
        var healerL2W = targetEntities.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

        var baseJob = Entities
            .WithAll<Healeable>()
            .WithDeallocateOnJobCompletion(healerPosition)
            .WithDeallocateOnJobCompletion(healerL2W)
            .ForEach((Entity entity, in Life life, in WorldRenderBounds bounds) =>
            {
                int heals = 0;
                for (var i = 0; i < healerPosition.Length; i++)
                {
                    var pos = math.mul(new float4(healerPosition[i].Value, 1), healerL2W[i].Value).xyz;
                    var sqdist = math.distancesq(pos, bounds.Value.Center);

                    if (sqdist < healDistance * healDistance)
                    {
                        heals++;
                    }
                }

                if (heals > 0)
                {
                    var currentHealth = math.min(life.amount + healAmmount * deltaTime * heals, life.maxAmount);
                    commandBuffer.SetComponent(0, entity, new Life {amount = currentHealth, maxAmount = life.maxAmount});
                }
                
            }).Schedule(inputDeps);
        buffer.AddJobHandleForProducer(baseJob);
        return baseJob;
    }
}