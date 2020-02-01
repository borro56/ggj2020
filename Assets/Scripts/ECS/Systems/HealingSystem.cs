using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class HealingSystem : JobComponentSystem
{
    EntityQuery targetEntities;
    private EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        targetEntities = GetEntityQuery(typeof(Life), ComponentType.ReadOnly<WorldRenderBounds>());
        base.OnCreate();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();

        var healDistance = HealerGlobal.Instance.Distance;
        var healAmmount = HealerGlobal.Instance.Amount;
        var deltaTime = Time.DeltaTime;
        // get entity references
        var targetsPosition = targetEntities.ToComponentDataArray<WorldRenderBounds>(Allocator.TempJob);
        var targetsLife = targetEntities.ToComponentDataArray<Life>(Allocator.TempJob);
        var targetsRef = targetEntities.ToEntityArray(Allocator.TempJob);

        var baseJob = Entities
            .WithDeallocateOnJobCompletion(targetsPosition)
            .WithAll<Healer>()
            .ForEach((in LocalToWorld l2w, in Translation tran) =>
            {
                for (var i = 0; i < targetsPosition.Length; i++)
                {
                    var pos = math.mul(new float4(tran.Value, 1), l2w.Value).xyz;
                    var targetBound = targetsPosition[i].Value;

                    var sqdist = math.distancesq(pos, targetBound.Center);
                    //Debug.Log(pos);
                    if (sqdist < healDistance * healDistance)
                    {
                        var currentHealth = math.min(targetsLife[i].amount + healAmmount * deltaTime, targetsLife[i].maxAmount);
                        commandBuffer.SetComponent(0, targetsRef[i], new Life() {amount = currentHealth, maxAmount = targetsLife[i].maxAmount});
                    }
                }
            }).Schedule(inputDeps);
        buffer.AddJobHandleForProducer(baseJob);
        return baseJob;
    }
}