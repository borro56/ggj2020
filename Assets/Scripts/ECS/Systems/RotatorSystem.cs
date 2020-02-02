using ECS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
    public class RotatorSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            return Entities
                .ForEach((ref Rotation rot, in Rotator rotator) =>
                    {
                        rot.Value = math.mul(rot.Value, quaternion.Euler(0, rotator.velocity * deltaTime, 0));
                    }).Schedule(inputDeps);
        }
    }
}