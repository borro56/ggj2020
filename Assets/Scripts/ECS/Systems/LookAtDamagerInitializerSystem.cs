using ECS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class LookAtDamagerInitializerSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var command = buffer.CreateCommandBuffer().ToConcurrent();
        var job = Entities
            .WithAll<LookAtDamagerInitializer>()
            .ForEach((Entity e , in Rotation rotation ) =>
            {
                command.AddComponent(0, e, new LookAtDamager() {initialRotation = rotation.Value});
                command.RemoveComponent(0, e, typeof(LookAtDamagerInitializer));
            }).Schedule(inputDeps);
        
        buffer.AddJobHandleForProducer(job);
        return job;
    }
}