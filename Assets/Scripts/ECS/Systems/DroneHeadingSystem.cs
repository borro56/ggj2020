using System.Diagnostics.Contracts;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
    public class DroneHeadingSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var healerContainer = GetComponentDataFromEntity<Healer>(true);
            var translationContainer = GetComponentDataFromEntity<Translation>(true);
            var velocityContainer = GetComponentDataFromEntity<Velocity>(true);
            var deltaTime = Time.DeltaTime;
            return Entities
                .WithAll<DroneHeading>()
                .WithReadOnly(healerContainer)
                .WithReadOnly(translationContainer)
                .WithReadOnly(velocityContainer)
                .ForEach((ref Rotation rotation, in Parent parent, in Translation trans, in LocalToWorld l2w) =>
                {
                    var pos = math.mul(l2w.Value, new float4(trans.Value, 1)).xyz;
                    var healingTarget = healerContainer[parent.Value].target;
                    var velocity = velocityContainer[parent.Value];
                    
                    float3 target = float3.zero;

                    if (healingTarget != Entity.Null)
                    {
                        target = translationContainer[healingTarget].Value;
                    }
                    else if(math.lengthsq(velocity.Value) > 0.1f)
                    {
                        target = pos + velocity.Value;
                    }
                    else
                    {
                        return;
                    }

                    var dir = target - pos;
                    
                        var targetRotation = quaternion.LookRotation(dir, new float3(0, 1, 0));
                    
                    rotation.Value = math.slerp(
                        rotation.Value
                        , targetRotation
                        , deltaTime * 10);

                    
                }).Schedule(inputDeps);
        }
    }
}