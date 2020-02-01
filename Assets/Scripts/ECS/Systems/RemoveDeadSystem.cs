using ECS.Systems;
using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(DamageSystem))]
public class RemoveDeadSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem bufferSystem;
    
    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var buffer = bufferSystem.CreateCommandBuffer().ToConcurrent();

        var jobHandle = Entities.ForEach((Entity entity, in Life life) =>
        {
            if(life.amount <= 0)
                buffer.DestroyEntity(0, entity);
        }).Schedule(inputDeps);
        
        bufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}