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
        targetEntities = GetEntityQuery(ComponentType.ReadOnly<Healer>());
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();

        var healDistance = HealerGlobal.Instance.Distance;
        var healAmmount = HealerGlobal.Instance.Amount;
        var deltaTime = Time.DeltaTime;
        
        var healers = targetEntities.ToComponentDataArray<Healer>(Allocator.TempJob);

        var baseJob = Entities
            .WithAll<Healeable>()
            .WithDeallocateOnJobCompletion(healers)
            .ForEach((Entity entity, in Life life, in WorldRenderBounds bounds) =>
            {
                if (life.amount >= life.maxAmount) return;
                
                int heals = 0;
                for (var i = 0; i < healers.Length; i++)
                {
                    if (healers[i].target == entity)
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