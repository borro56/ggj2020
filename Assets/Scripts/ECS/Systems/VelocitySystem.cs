using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public  class VelocitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;
        return Entities.ForEach((ref Translation translation, ref Velocity velocity) =>
        {
            velocity.Value -= velocity.Value * velocity.damping * deltaTime;
            translation.Value += velocity.Value * deltaTime;
        }).Schedule(inputDeps);
    }
}