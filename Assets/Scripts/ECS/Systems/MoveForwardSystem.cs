using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : JobComponentSystem
{ 
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;

        return Entities.ForEach((ref Translation tran, in Rotation rot, in MoveForward moveForward) =>
            {
                tran.Value += math.forward(rot.Value) * moveForward.speed * deltaTime;
            }).Schedule(inputDeps);
    }
}